using System.Data;
using System.Globalization;
using AsliHalwaiWebApp.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AsliHalwai.DbMigrator;

public static class Migrator
{
    public static Task<int> RunAsync(MigratorOptions options)
        => Task.FromResult(Run(options));

    private static int Run(MigratorOptions options)
    {
        var targetConnectionString = BuildTargetConnectionString(options.Target);
        var targetPath = new SqliteConnectionStringBuilder(targetConnectionString).DataSource;
        var sourceDisplay = Redact(options.Source!);

        Console.WriteLine("AsliHalwai SQL Server -> SQLite migrator");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"Source : {sourceDisplay}");
        Console.WriteLine($"Target : {targetPath}");
        Console.WriteLine();

        if (options.VerifyOnly)
            return VerifyOnly(options, targetConnectionString, targetPath);

        if (options.DryRun)
            return DryRun(options, targetConnectionString, targetPath);

        EnsureTargetSchema(targetConnectionString, targetPath);

        using var context = CreateContext(targetConnectionString);
        var modelTables = GetModelTables(context);

        using var source = new SqlConnection(options.Source);
        source.Open();

        using var sourceTransaction = source.BeginTransaction(IsolationLevel.RepeatableRead);

        var plans = BuildPlans(source, sourceTransaction, modelTables);
        PrintPlan(plans);

        if (plans.Any(p => p.Errors.Count > 0))
        {
            Console.Error.WriteLine("Source validation failed. No data was written.");
            return 2;
        }

        var targetCounts = GetSqliteCounts(targetConnectionString, modelTables.Select(t => t.Name));
        var nonEmptyTarget = targetCounts.Where(kv => kv.Value > 0).ToList();

        if (nonEmptyTarget.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("The SQLite target already contains data:");
            PrintCounts(nonEmptyTarget, Console.Out);

            if (!options.ReplaceExisting)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine(
                    "Refusing to modify a non-empty target. Back it up, then rerun with " +
                    "--replace-existing --yes if you really want to replace its contents.");
                return 2;
            }

            if (!options.Yes)
            {
                Console.WriteLine();
                Console.Write("Type MIGRATE to back up and clear the target, then continue: ");
                if (!string.Equals(Console.ReadLine(), "MIGRATE", StringComparison.Ordinal))
                {
                    Console.WriteLine("Aborted. The target was not changed.");
                    return 3;
                }
            }

            BackupTarget(targetConnectionString, targetPath);
        }

        CopyData(options, source, sourceTransaction, plans, targetConnectionString, targetPath);
        sourceTransaction.Commit();

        Console.WriteLine();
        Console.WriteLine("Verifying imported record counts...");
        var verifyResult = VerifyCountsAndRelationships(
            source,
            sourceTransaction: null,
            plans,
            targetConnectionString,
            printTable: true);

        if (verifyResult != 0)
        {
            Console.Error.WriteLine("Verification failed. Do not start the application yet.");
            return 2;
        }

        Console.WriteLine();
        Console.WriteLine("SUCCESS: SQL Server data was imported into SQLite.");
        Console.WriteLine("Original SQL Server database was only read; it was not modified.");
        Console.WriteLine($"SQLite database: {targetPath}");
        Console.WriteLine("Next: start the application with Database__AutoMigrate=false.");
        return 0;
    }

    private static void EnsureTargetSchema(string targetConnectionString, string targetPath)
    {
        var targetExisted = File.Exists(targetPath);

        using var context = CreateContext(targetConnectionString);

        if (!targetExisted)
        {
            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            Console.WriteLine("Creating the SQLite schema from EF Core migrations...");
            context.Database.Migrate();
            Console.WriteLine("SQLite schema created.");
            return;
        }

        var pending = context.Database.GetPendingMigrations().ToList();
        if (pending.Count > 0)
        {
            throw new InvalidOperationException(
                "The target SQLite database has pending migrations: " +
                string.Join(", ", pending) +
                ". Back it up and run 'dotnet ef database update' from the web project first. " +
                "The migrator will not apply migrations automatically.");
        }

        Console.WriteLine("SQLite schema is up to date.");
    }

    private static int DryRun(MigratorOptions options, string targetConnectionString, string targetPath)
    {
        using var context = CreateContext(targetConnectionString);
        var modelTables = GetModelTables(context);

        using var source = new SqlConnection(options.Source);
        source.Open();
        using var sourceTransaction = source.BeginTransaction(IsolationLevel.RepeatableRead);

        var plans = BuildPlans(source, sourceTransaction, modelTables);
        PrintPlan(plans);

        Console.WriteLine();
        Console.WriteLine($"Target exists: {File.Exists(targetPath)}");

        if (File.Exists(targetPath))
        {
            var counts = GetSqliteCounts(targetConnectionString, modelTables.Select(t => t.Name));
            var nonEmpty = counts.Where(kv => kv.Value > 0).ToList();
            if (nonEmpty.Count > 0)
            {
                Console.WriteLine("Target is non-empty:");
                PrintCounts(nonEmpty, Console.Out);
                Console.WriteLine("A real import needs --replace-existing --yes.");
            }
            else
            {
                Console.WriteLine("Target is empty.");
            }
        }
        else
        {
            Console.WriteLine("A real import will create the database and apply migrations.");
        }

        return plans.Any(p => p.Errors.Count > 0) ? 2 : 0;
    }

    private static int VerifyOnly(MigratorOptions options, string targetConnectionString, string targetPath)
    {
        if (!File.Exists(targetPath))
            throw new InvalidOperationException($"Target SQLite database not found: {targetPath}");

        using var context = CreateContext(targetConnectionString);
        var modelTables = GetModelTables(context);

        using var source = new SqlConnection(options.Source);
        source.Open();
        using var sourceTransaction = source.BeginTransaction(IsolationLevel.RepeatableRead);

        var plans = BuildPlans(source, sourceTransaction, modelTables);
        PrintPlan(plans);

        if (plans.Any(p => p.Errors.Count > 0))
            return 2;

        Console.WriteLine();
        var result = VerifyCountsAndRelationships(
            source,
            sourceTransaction,
            plans,
            targetConnectionString,
            printTable: true);

        return result;
    }

    private static void CopyData(
        MigratorOptions options,
        SqlConnection source,
        SqlTransaction sourceTransaction,
        IReadOnlyList<TablePlan> plans,
        string targetConnectionString,
        string targetPath)
    {
        using var target = new SqliteConnection(targetConnectionString);
        target.Open();

        ExecuteNonQuery(target, "PRAGMA foreign_keys=OFF; PRAGMA busy_timeout=10000;");

        using var targetTransaction = target.BeginTransaction();

        if (options.ReplaceExisting)
        {
            Console.WriteLine();
            Console.WriteLine("Clearing the existing SQLite target (a backup was created)...");

            foreach (var plan in plans.OrderByDescending(p => p.TableName))
                ExecuteNonQuery(target, targetTransaction, $"DELETE FROM {QuoteSqlite(plan.TableName)};");
        }

        Console.WriteLine();
        Console.WriteLine("Copying tables...");

        foreach (var plan in plans.Where(p => p.SourceExists))
        {
            if (plan.Columns.Count == 0)
            {
                Console.WriteLine($"  {plan.TableName,-35} skipped (no insertable columns)");
                continue;
            }

            var selectSql = BuildSelectSql(plan);
            var insertSql = BuildInsertSql(plan);

            using var select = source.CreateCommand();
            select.Transaction = sourceTransaction;
            select.CommandText = selectSql;
            select.CommandTimeout = 0;

            using var reader = select.ExecuteReader();

            using var insert = target.CreateCommand();
            insert.Transaction = targetTransaction;
            insert.CommandText = insertSql;
            insert.CommandTimeout = 0;

            for (var i = 0; i < plan.Columns.Count; i++)
            {
                var parameter = insert.CreateParameter();
                parameter.ParameterName = $"$p{i}";
                insert.Parameters.Add(parameter);
            }

            insert.Prepare();

            long copied = 0;
            while (reader.Read())
            {
                copied++;

                for (var i = 0; i < plan.Columns.Count; i++)
                {
                    var column = plan.Columns[i];
                    var raw = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                    insert.Parameters[i].Value = ConvertValue(
                        raw,
                        column.TargetType,
                        column.IsNullable,
                        plan.TableName,
                        column.TargetColumn,
                        copied);
                }

                insert.ExecuteNonQuery();
            }

            if (copied != plan.SourceRowCount)
            {
                throw new InvalidOperationException(
                    $"Source row count changed while copying {plan.TableName}: " +
                    $"expected {plan.SourceRowCount}, read {copied}. " +
                    "Stop the application using SQL Server and retry.");
            }

            Console.WriteLine($"  {plan.TableName,-35} {copied,8} rows");
        }

        Console.WriteLine();
        Console.WriteLine("Checking SQLite foreign keys and integrity...");
        CheckForeignKeys(target, targetTransaction);
        CheckIntegrity(target, targetTransaction);

        targetTransaction.Commit();

        ResetAutoIncrement(target);
        EnableForeignKeys(target);
    }

    private static int VerifyCountsAndRelationships(
        SqlConnection source,
        SqlTransaction? sourceTransaction,
        IReadOnlyList<TablePlan> plans,
        string targetConnectionString,
        bool printTable)
    {
        var targetCounts = GetSqliteCounts(
            targetConnectionString,
            plans.Select(p => p.TableName));

        var rows = new List<(string Table, long Source, long Target)>();
        var mismatches = 0;

        foreach (var plan in plans)
        {
            var targetCount = targetCounts.TryGetValue(plan.TableName, out var count) ? count : -1;
            rows.Add((plan.TableName, plan.SourceExists ? plan.SourceRowCount : -1, targetCount));

            if (plan.SourceExists && targetCount != plan.SourceRowCount)
                mismatches++;
        }

        if (printTable)
        {
            Console.WriteLine();
            Console.WriteLine($"{"Table",-35} {"SQL Server",12} {"SQLite",12} Status");
            Console.WriteLine(new string('-', 75));

            foreach (var row in rows)
            {
                var status = row.Source < 0
                    ? "source missing"
                    : row.Source == row.Target ? "OK" : "MISMATCH";

                Console.WriteLine(
                    $"{row.Table,-35} {row.Source,12} {row.Target,12} {status}");
            }
        }

        using var target = new SqliteConnection(targetConnectionString);
        target.Open();

        var fkErrors = GetForeignKeyErrors(target);
        if (fkErrors.Count > 0)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("SQLite foreign key check failed:");
            foreach (var error in fkErrors.Take(20))
                Console.Error.WriteLine("  " + error);
        }
        else
        {
            Console.WriteLine("Foreign key check: OK");
        }

        var integrity = GetIntegrityResult(target);
        Console.WriteLine($"Integrity check : {integrity}");

        if (mismatches > 0)
        {
            Console.Error.WriteLine($"{mismatches} table(s) have different row counts.");
            return 2;
        }

        if (fkErrors.Count > 0 || !integrity.Equals("ok", StringComparison.OrdinalIgnoreCase))
            return 2;

        return 0;
    }

    private static IReadOnlyList<TablePlan> BuildPlans(
        SqlConnection source,
        SqlTransaction sourceTransaction,
        IReadOnlyList<ModelTable> modelTables)
    {
        var sourceTables = GetSourceTables(source, sourceTransaction);
        var plans = new List<TablePlan>();

        foreach (var modelTable in modelTables)
        {
            var plan = new TablePlan
            {
                TableName = modelTable.Name
            };

            if (!sourceTables.TryGetValue(modelTable.Name, out var sourceTable))
            {
                plan.Warnings.Add("Source table not found; target table will remain empty.");
                plans.Add(plan);
                continue;
            }

            plan.SourceExists = true;
            plan.SourceSchema = sourceTable.Schema;
            plan.SourceTableName = sourceTable.Name;

            var sourceColumns = GetSourceColumns(
                source, sourceTransaction, sourceTable.Schema, sourceTable.Name);

            var sourceColumnSet = new HashSet<string>(
                sourceColumns, StringComparer.OrdinalIgnoreCase);

            foreach (var column in modelTable.Columns)
            {
                if (sourceColumnSet.Contains(column.Name))
                {
                    plan.Columns.Add(new ColumnPlan
                    {
                        TargetColumn = column.Name,
                        SourceColumn = column.Name,
                        TargetType = column.ClrType,
                        IsNullable = column.IsNullable,
                        IsPrimaryKey = column.IsPrimaryKey
                    });
                }
                else if (column.IsNullable)
                {
                    plan.Warnings.Add(
                        $"Source column '{column.Name}' is missing; NULL will be inserted.");
                    plan.Columns.Add(new ColumnPlan
                    {
                        TargetColumn = column.Name,
                        SourceColumn = null,
                        TargetType = column.ClrType,
                        IsNullable = true,
                        IsPrimaryKey = column.IsPrimaryKey
                    });
                }
                else
                {
                    plan.Errors.Add(
                        $"Source column '{column.Name}' is missing but the target column is NOT NULL.");
                }
            }

            var modelColumnSet = new HashSet<string>(
                modelTable.Columns.Select(c => c.Name), StringComparer.OrdinalIgnoreCase);

            foreach (var extra in sourceColumns.Where(c => !modelColumnSet.Contains(c)))
                plan.Warnings.Add($"Source column '{extra}' is not mapped by EF and will be ignored.");

            plan.SourceRowCount = CountSourceRows(
                source, sourceTransaction, sourceTable.Schema, sourceTable.Name);

            plan.PrimaryKey.AddRange(
                modelTable.PrimaryKey.Where(pk =>
                    plan.Columns.Any(c => c.TargetColumn.Equals(pk, StringComparison.OrdinalIgnoreCase))));

            plans.Add(plan);
        }

        return plans;
    }

    private static void PrintPlan(IReadOnlyList<TablePlan> plans)
    {
        Console.WriteLine();
        Console.WriteLine("Migration plan:");

        foreach (var plan in plans)
        {
            var source = plan.SourceExists
                ? $"{plan.SourceSchema}.{plan.SourceTableName}"
                : "(missing)";

            Console.WriteLine(
                $"  {plan.TableName,-35} <- {source,-40} {plan.SourceRowCount,8} rows");

            foreach (var warning in plan.Warnings)
                Console.WriteLine($"       WARNING: {warning}");

            foreach (var error in plan.Errors)
                Console.Error.WriteLine($"       ERROR  : {error}");
        }
    }

    private static IReadOnlyList<ModelTable> GetModelTables(AppDbContext context)
    {
        var tables = new List<ModelTable>();

        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
                continue;

            var storeObject = StoreObjectIdentifier.Table(tableName, null);
            var primaryKeyProperties = entityType.FindPrimaryKey()?.Properties
                .Select(p => p.GetColumnName(storeObject))
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();

            var columns = new List<ModelColumn>();

            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName(storeObject);
                if (string.IsNullOrWhiteSpace(columnName))
                    continue;

                // Computed columns cannot be inserted.
                if (property.ValueGenerated == ValueGenerated.OnAddOrUpdate)
                    continue;

                columns.Add(new ModelColumn(
                    columnName,
                    property.ClrType,
                    property.IsNullable,
                    primaryKeyProperties.Contains(columnName)));
            }

            tables.Add(new ModelTable(
                tableName,
                columns,
                primaryKeyProperties.ToList()));
        }

        return tables
            .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static Dictionary<string, SourceTable> GetSourceTables(
        SqlConnection source,
        SqlTransaction transaction)
    {
        var tables = new Dictionary<string, SourceTable>(StringComparer.OrdinalIgnoreCase);

        using var command = source.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            "SELECT TABLE_SCHEMA, TABLE_NAME " +
            "FROM INFORMATION_SCHEMA.TABLES " +
            "WHERE TABLE_TYPE = 'BASE TABLE'";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var schema = reader.GetString(0);
            var name = reader.GetString(1);

            if (name.Equals("sysdiagrams", StringComparison.OrdinalIgnoreCase))
                continue;

            // Prefer dbo if the same table name exists in more than one schema.
            if (!tables.TryGetValue(name, out var existing) ||
                (!existing.Schema.Equals("dbo", StringComparison.OrdinalIgnoreCase) &&
                 schema.Equals("dbo", StringComparison.OrdinalIgnoreCase)))
            {
                tables[name] = new SourceTable(schema, name);
            }
        }

        return tables;
    }

    private static List<string> GetSourceColumns(
        SqlConnection source,
        SqlTransaction transaction,
        string schema,
        string table)
    {
        var columns = new List<string>();

        using var command = source.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS " +
            "WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table " +
            "ORDER BY ORDINAL_POSITION";
        command.Parameters.AddWithValue("@schema", schema);
        command.Parameters.AddWithValue("@table", table);

        using var reader = command.ExecuteReader();
        while (reader.Read())
            columns.Add(reader.GetString(0));

        return columns;
    }

    private static long CountSourceRows(
        SqlConnection source,
        SqlTransaction transaction,
        string schema,
        string table)
    {
        using var command = source.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"SELECT COUNT_BIG(*) FROM {QuoteSqlServer(schema)}.{QuoteSqlServer(table)}";

        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static string BuildSelectSql(TablePlan plan)
    {
        var columns = plan.Columns.Select(column =>
            column.SourceColumn is null
                ? $"NULL AS {QuoteSqlServer(column.TargetColumn)}"
                : QuoteSqlServer(column.SourceColumn));

        var sql =
            $"SELECT {string.Join(", ", columns)} " +
            $"FROM {QuoteSqlServer(plan.SourceSchema!)}.{QuoteSqlServer(plan.SourceTableName!)}";

        var orderColumns = plan.PrimaryKey
            .Where(pk => plan.Columns.Any(c =>
                c.TargetColumn.Equals(pk, StringComparison.OrdinalIgnoreCase) &&
                c.SourceColumn is not null))
            .Select(QuoteSqlServer)
            .ToList();

        if (orderColumns.Count > 0)
            sql += $" ORDER BY {string.Join(", ", orderColumns)}";

        return sql;
    }

    private static string BuildInsertSql(TablePlan plan)
    {
        var columns = plan.Columns.Select(c => QuoteSqlite(c.TargetColumn));
        var parameters = plan.Columns.Select((_, i) => $"$p{i}");

        return
            $"INSERT INTO {QuoteSqlite(plan.TableName)} " +
            $"({string.Join(", ", columns)}) " +
            $"VALUES ({string.Join(", ", parameters)});";
    }

    private static Dictionary<string, long> GetSqliteCounts(
        string targetConnectionString,
        IEnumerable<string> tableNames)
    {
        var counts = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        using var connection = new SqliteConnection(targetConnectionString);
        connection.Open();

        foreach (var tableName in tableNames)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM {QuoteSqlite(tableName)};";
            counts[tableName] = Convert.ToInt64(
                command.ExecuteScalar(), CultureInfo.InvariantCulture);
        }

        return counts;
    }

    private static void BackupTarget(string targetConnectionString, string targetPath)
    {
        var backupDirectory = Path.Combine(
            Path.GetDirectoryName(targetPath)!, "backups");
        Directory.CreateDirectory(backupDirectory);

        var backupPath = Path.Combine(
            backupDirectory,
            $"{Path.GetFileNameWithoutExtension(targetPath)}_" +
            $"{DateTime.Now:yyyyMMdd_HHmmss}.db");

        var sourceBuilder = new SqliteConnectionStringBuilder(targetConnectionString);
        var destinationBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = backupPath,
            Pooling = false
        };

        using var source = new SqliteConnection(sourceBuilder.ToString());
        using var destination = new SqliteConnection(destinationBuilder.ToString());

        source.Open();
        destination.Open();
        source.BackupDatabase(destination);

        Console.WriteLine($"Target backup created: {backupPath}");
    }

    private static void CheckForeignKeys(SqliteConnection connection, SqliteTransaction transaction)
    {
        var errors = GetForeignKeyErrors(connection, transaction);

        if (errors.Count > 0)
        {
            var message =
                "Foreign key check failed. The target was NOT committed. " +
                "First 20 problem rows:" + Environment.NewLine +
                string.Join(Environment.NewLine, errors.Take(20));

            throw new InvalidOperationException(message);
        }
    }

    private static List<string> GetForeignKeyErrors(
        SqliteConnection connection,
        SqliteTransaction? transaction = null)
    {
        var errors = new List<string>();

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "PRAGMA foreign_key_check;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var table = reader.GetString(0);
            var rowId = reader.IsDBNull(1) ? "?" : reader.GetValue(1).ToString();
            var parent = reader.GetString(2);
            errors.Add($"table={table} rowid={rowId} parent={parent}");
        }

        return errors;
    }

    private static void CheckIntegrity(SqliteConnection connection, SqliteTransaction transaction)
    {
        var integrity = GetIntegrityResult(connection, transaction);
        if (!integrity.Equals("ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"SQLite integrity check failed: {integrity}. The target was NOT committed.");
        }
    }

    private static string GetIntegrityResult(
        SqliteConnection connection,
        SqliteTransaction? transaction = null)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "PRAGMA integrity_check;";
        return command.ExecuteScalar()?.ToString() ?? "unknown";
    }

    private static void ResetAutoIncrement(SqliteConnection connection)
    {
        var tables = new List<string>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText =
                "SELECT name FROM sqlite_master " +
                "WHERE type = 'table' AND sql LIKE '%AUTOINCREMENT%';";

            using var reader = command.ExecuteReader();
            while (reader.Read())
                tables.Add(reader.GetString(0));
        }

        using var transaction = connection.BeginTransaction();

        foreach (var table in tables)
        {
            long maxId;

            using (var maxCommand = connection.CreateCommand())
            {
                maxCommand.Transaction = transaction;
                maxCommand.CommandText = $"SELECT COALESCE(MAX(rowid), 0) FROM {QuoteSqlite(table)};";
                maxId = Convert.ToInt64(
                    maxCommand.ExecuteScalar(), CultureInfo.InvariantCulture);
            }

            using (var delete = connection.CreateCommand())
            {
                delete.Transaction = transaction;
                delete.CommandText = "DELETE FROM sqlite_sequence WHERE name = $name;";
                delete.Parameters.AddWithValue("$name", table);
                delete.ExecuteNonQuery();
            }

            using (var insert = connection.CreateCommand())
            {
                insert.Transaction = transaction;
                insert.CommandText =
                    "INSERT INTO sqlite_sequence(name, seq) VALUES ($name, $seq);";
                insert.Parameters.AddWithValue("$name", table);
                insert.Parameters.AddWithValue("$seq", maxId);
                insert.ExecuteNonQuery();
            }
        }

        transaction.Commit();
    }

    private static void EnableForeignKeys(SqliteConnection connection)
    {
        ExecuteNonQuery(connection, "PRAGMA foreign_keys=ON;");
    }

    private static void ExecuteNonQuery(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void ExecuteNonQuery(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sql)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static object ConvertValue(
        object raw,
        Type targetType,
        bool isNullable,
        string table,
        string column,
        long rowNumber)
    {
        if (raw is null or DBNull)
        {
            if (isNullable)
                return DBNull.Value;

            throw new InvalidOperationException(
                $"NULL in source {table}.{column} at row {rowNumber}, " +
                "but the target column is NOT NULL.");
        }

        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (type == typeof(string))
            return Convert.ToString(raw, CultureInfo.InvariantCulture) ?? string.Empty;

        if (type == typeof(int))
            return Convert.ToInt32(raw, CultureInfo.InvariantCulture);

        if (type == typeof(long))
            return Convert.ToInt64(raw, CultureInfo.InvariantCulture);

        if (type == typeof(short))
            return Convert.ToInt16(raw, CultureInfo.InvariantCulture);

        if (type == typeof(byte))
            return Convert.ToByte(raw, CultureInfo.InvariantCulture);

        if (type == typeof(sbyte))
            return Convert.ToSByte(raw, CultureInfo.InvariantCulture);

        if (type == typeof(ushort))
            return Convert.ToUInt16(raw, CultureInfo.InvariantCulture);

        if (type == typeof(uint))
            return Convert.ToUInt32(raw, CultureInfo.InvariantCulture);

        if (type == typeof(ulong))
            return Convert.ToUInt64(raw, CultureInfo.InvariantCulture);

        if (type == typeof(double))
            return Convert.ToDouble(raw, CultureInfo.InvariantCulture);

        if (type == typeof(float))
            return Convert.ToSingle(raw, CultureInfo.InvariantCulture);

        if (type == typeof(decimal))
            return Convert.ToDecimal(raw, CultureInfo.InvariantCulture);

        if (type == typeof(bool))
        {
            if (raw is bool boolean)
                return boolean;

            if (raw is string text)
                return text is "1" or "true" or "True" or "TRUE";

            return Convert.ToInt64(raw, CultureInfo.InvariantCulture) != 0;
        }

        if (type == typeof(DateTime))
        {
            return raw switch
            {
                DateTime dt => DateTime.SpecifyKind(dt, DateTimeKind.Unspecified),
                DateTimeOffset dto => DateTime.SpecifyKind(dto.DateTime, DateTimeKind.Unspecified),
                DateOnly date => date.ToDateTime(TimeOnly.MinValue),
                string text => DateTime.Parse(
                    text, CultureInfo.InvariantCulture, DateTimeStyles.None),
                _ => Convert.ToDateTime(raw, CultureInfo.InvariantCulture)
            };
        }

        if (type == typeof(DateOnly))
        {
            return raw switch
            {
                DateOnly date => date,
                DateTime dt => DateOnly.FromDateTime(dt),
                DateTimeOffset dto => DateOnly.FromDateTime(dto.DateTime),
                string text => DateOnly.Parse(text, CultureInfo.InvariantCulture),
                _ => DateOnly.FromDateTime(
                    Convert.ToDateTime(raw, CultureInfo.InvariantCulture))
            };
        }

        if (type == typeof(TimeOnly))
        {
            return raw switch
            {
                TimeOnly time => time,
                TimeSpan span => TimeOnly.FromTimeSpan(span),
                DateTime dt => TimeOnly.FromDateTime(dt),
                DateTimeOffset dto => TimeOnly.FromDateTime(dto.DateTime),
                string text => TimeOnly.Parse(text, CultureInfo.InvariantCulture),
                _ => TimeOnly.FromTimeSpan(
                    (TimeSpan)Convert.ChangeType(raw, typeof(TimeSpan), CultureInfo.InvariantCulture))
            };
        }

        if (type == typeof(TimeSpan))
        {
            return raw switch
            {
                TimeSpan span => span,
                TimeOnly time => time.ToTimeSpan(),
                string text => TimeSpan.Parse(text, CultureInfo.InvariantCulture),
                _ => (TimeSpan)Convert.ChangeType(
                    raw, typeof(TimeSpan), CultureInfo.InvariantCulture)
            };
        }

        if (type == typeof(Guid))
        {
            return raw switch
            {
                Guid guid => guid,
                string text => Guid.Parse(text),
                _ => new Guid((byte[])raw)
            };
        }

        if (type == typeof(byte[]))
            return (byte[])raw;

        if (type == typeof(char))
            return Convert.ToChar(raw, CultureInfo.InvariantCulture);

        if (type.IsEnum)
            return Convert.ToInt32(raw, CultureInfo.InvariantCulture);

        return raw;
    }

    private static AppDbContext CreateContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    private static string BuildTargetConnectionString(string target)
    {
        if (target.Contains('='))
        {
            var builder = new SqliteConnectionStringBuilder(target);

            if (string.IsNullOrWhiteSpace(builder.DataSource))
                throw new ArgumentException("The target connection string has no Data Source.");

            if (!Path.IsPathRooted(builder.DataSource))
            {
                builder.DataSource = Path.GetFullPath(builder.DataSource);
            }

            var directory = Path.GetDirectoryName(builder.DataSource);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            builder.ForeignKeys = true;
            builder.Pooling = false;
            return builder.ToString();
        }

        var path = Path.GetFullPath(target);
        var parent = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(parent))
            Directory.CreateDirectory(parent);

        return new SqliteConnectionStringBuilder
        {
            DataSource = path,
            ForeignKeys = true,
            Pooling = false
        }.ToString();
    }

    private static void PrintCounts(
        IEnumerable<KeyValuePair<string, long>> counts,
        TextWriter writer)
    {
        foreach (var pair in counts.OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
            writer.WriteLine($"  {pair.Key,-35} {pair.Value,8}");
    }

    private static string QuoteSqlServer(string identifier)
        => "[" + identifier.Replace("]", "]]") + "]";

    private static string QuoteSqlite(string identifier)
        => "\"" + identifier.Replace("\"", "\"\"") + "\"";

    private static string Redact(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        if (!string.IsNullOrEmpty(builder.Password))
            builder.Password = "***";

        return builder.ConnectionString;
    }

    private sealed record ModelColumn(
        string Name,
        Type ClrType,
        bool IsNullable,
        bool IsPrimaryKey);

    private sealed record ModelTable(
        string Name,
        IReadOnlyList<ModelColumn> Columns,
        IReadOnlyList<string> PrimaryKey);

    private sealed record SourceTable(string Schema, string Name);

    private sealed class TablePlan
    {
        public string TableName { get; init; } = "";
        public bool SourceExists { get; set; }
        public string? SourceSchema { get; set; }
        public string? SourceTableName { get; set; }
        public List<ColumnPlan> Columns { get; } = new();
        public List<string> PrimaryKey { get; } = new();
        public long SourceRowCount { get; set; }
        public List<string> Warnings { get; } = new();
        public List<string> Errors { get; } = new();
    }

    private sealed class ColumnPlan
    {
        public string TargetColumn { get; init; } = "";
        public string? SourceColumn { get; init; }
        public Type TargetType { get; init; } = typeof(object);
        public bool IsNullable { get; init; }
        public bool IsPrimaryKey { get; init; }
    }
}
