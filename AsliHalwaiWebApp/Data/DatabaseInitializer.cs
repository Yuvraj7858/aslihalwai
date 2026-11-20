using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Data;

/// <summary>
/// Applies EF Core migrations at startup with production safeguards:
///  - a missing database is provisioned automatically;
///  - an existing database is only migrated when Database:AutoMigrate is true;
///  - an existing database is backed up before any migration is applied.
/// </summary>
public static class DatabaseInitializer
{
    public static void Initialize(
        AppDbContext db,
        IConfiguration configuration,
        string contentRootPath,
        ILogger logger)
    {
        var connectionString = db.Database.GetConnectionString()
            ?? throw new InvalidOperationException("Database connection string is not configured.");

        var databasePath = SqliteDatabase.GetDataSource(connectionString);
        if (databasePath.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            ApplyMigrations(db, logger);
            return;
        }

        var autoMigrate = configuration.GetValue("Database:AutoMigrate", false);
        var seedReferenceData = configuration.GetValue("Database:SeedReferenceData", true);
        var backupDirectory = SqliteDatabase.ResolvePath(
            configuration["Database:BackupDirectory"] ?? "Data/backups",
            contentRootPath);

        var databaseExisted = File.Exists(databasePath);

        if (!databaseExisted)
        {
            logger.LogInformation(
                "SQLite database not found at {Path}. Creating it from EF Core migrations.",
                databasePath);

            ApplyMigrations(db, logger);
        }
        else
        {
            var pending = db.Database.GetPendingMigrations().ToList();

            if (pending.Count > 0)
            {
                if (!autoMigrate)
                {
                    logger.LogWarning(
                        "The SQLite database has {Count} pending migration(s): {Migrations}. " +
                        "It was NOT changed. Back up the database, then run " +
                        "'dotnet ef database update' or set Database__AutoMigrate=true.",
                        pending.Count,
                        string.Join(", ", pending));
                }
                else
                {
                    var backupPath = CreateBackup(
                        connectionString, backupDirectory, logger);

                    logger.LogInformation(
                        "Applying {Count} pending migration(s) to {Path}. Backup: {Backup}",
                        pending.Count, databasePath, backupPath);

                    ApplyMigrations(db, logger);
                }
            }
        }

        EnableWal(db, logger);

        if (seedReferenceData)
        {
            var seeded = ReferenceDataSeeder.Seed(db);
            if (seeded.Count > 0)
            {
                logger.LogInformation(
                    "Seeded empty lookup tables: {Tables}",
                    string.Join(", ", seeded));
            }
        }

        logger.LogInformation("SQLite database ready at {Path}", databasePath);
    }

    private static void ApplyMigrations(AppDbContext db, ILogger logger)
    {
        var pending = db.Database.GetPendingMigrations().ToList();
        if (pending.Count == 0)
            return;

        db.Database.Migrate();
        logger.LogInformation(
            "Applied EF Core migration(s): {Migrations}",
            string.Join(", ", pending));
    }

    private static string CreateBackup(
        string connectionString,
        string backupDirectory,
        ILogger logger)
    {
        Directory.CreateDirectory(backupDirectory);

        var sourceBuilder = new SqliteConnectionStringBuilder(connectionString);
        var databaseName = Path.GetFileNameWithoutExtension(sourceBuilder.DataSource);
        var backupPath = Path.Combine(
            backupDirectory,
            $"{databaseName}_{DateTime.Now:yyyyMMdd_HHmmss}.db");

        var destinationBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = backupPath,
            Pooling = false
        };

        using var source = new SqliteConnection(sourceBuilder.ToString());
        using var destination = new SqliteConnection(destinationBuilder.ToString());

        source.Open();
        destination.Open();

        // SQLite's online backup API includes committed WAL content.
        source.BackupDatabase(destination);

        logger.LogInformation("Database backup created at {BackupPath}", backupPath);
        return backupPath;
    }

    private static void EnableWal(AppDbContext db, ILogger logger)
    {
        try
        {
            var connection = db.Database.GetDbConnection();
            var shouldClose = connection.State != System.Data.ConnectionState.Open;

            if (shouldClose)
                connection.Open();

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA journal_mode=WAL;";
                var mode = command.ExecuteScalar()?.ToString();

                if (!string.Equals(mode, "wal", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning(
                        "SQLite journal_mode is {Mode}; WAL could not be enabled. " +
                        "Concurrent writes will be more limited.",
                        mode);
                }
            }
            finally
            {
                if (shouldClose)
                    connection.Close();
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not enable SQLite WAL mode.");
        }
    }
}
