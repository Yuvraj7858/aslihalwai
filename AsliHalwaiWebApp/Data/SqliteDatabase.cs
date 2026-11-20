using Microsoft.Data.Sqlite;

namespace AsliHalwaiWebApp.Data;

/// <summary>
/// Resolves the SQLite database path relative to the application content root
/// and makes sure the directory exists before SQLite tries to open the file.
/// </summary>
public static class SqliteDatabase
{
    public static string ResolveConnectionString(string connectionString, string contentRootPath)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string 'constr' is missing. Set ConnectionStrings__constr.");

        var builder = new SqliteConnectionStringBuilder(connectionString);

        if (string.IsNullOrWhiteSpace(builder.DataSource))
            throw new InvalidOperationException(
                "SQLite connection string must contain a Data Source.");

        if (!IsSpecialDataSource(builder.DataSource))
        {
            if (!Path.IsPathRooted(builder.DataSource))
            {
                builder.DataSource = Path.GetFullPath(
                    Path.Combine(contentRootPath, builder.DataSource));
            }

            var directory = Path.GetDirectoryName(builder.DataSource);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);
        }

        // Foreign Keys=True is kept from the configured connection string.
        // Pooling reduces connection churn on a phone-class CPU.
        if (!builder.ContainsKey("Pooling"))
            builder.Pooling = true;

        return builder.ToString();
    }

    public static string GetDataSource(string connectionString)
    {
        return new SqliteConnectionStringBuilder(connectionString).DataSource;
    }

    public static string ResolvePath(string path, string contentRootPath)
    {
        if (string.IsNullOrWhiteSpace(path))
            path = "Data/backups";

        return Path.IsPathRooted(path)
            ? path
            : Path.GetFullPath(Path.Combine(contentRootPath, path));
    }

    private static bool IsSpecialDataSource(string dataSource)
    {
        return dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase)
            || dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase);
    }
}
