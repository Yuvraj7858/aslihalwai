using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AsliHalwaiWebApp.Data;

/// <summary>
/// Applies per-connection SQLite settings that EF Core does not set itself.
/// WAL mode is enabled once by <see cref="DatabaseInitializer"/> because it is
/// persisted in the database file.
/// </summary>
public sealed class SqliteConnectionInterceptor : DbConnectionInterceptor
{
    private const string Pragmas =
        "PRAGMA busy_timeout=10000;" +
        "PRAGMA foreign_keys=ON;" +
        "PRAGMA synchronous=NORMAL;";

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        Apply(connection);
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await ApplyAsync(connection, cancellationToken);
    }

    private static void Apply(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = Pragmas;
        command.ExecuteNonQuery();
    }

    private static async Task ApplyAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = Pragmas;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
