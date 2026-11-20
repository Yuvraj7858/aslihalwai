# AsliHalwai SQL Server → SQLite data migrator

One-time console tool that copies the existing SQL Server database into the
SQLite database used by `AsliHalwaiWebApp`.

The tool:

- **never writes to SQL Server** (the source is opened in a `RepeatableRead`
  transaction and only `SELECT` statements are issued);
- creates the SQLite schema from the app's EF Core migrations;
- copies every EF Core mapped table in primary-key order;
- refuses to touch a non-empty target unless `--replace-existing` is passed;
- creates a SQLite online backup before replacing a non-empty target;
- verifies every table's row count, `PRAGMA foreign_key_check`, and
  `PRAGMA integrity_check`;
- resets `sqlite_sequence` so new rows do not reuse old primary keys.

The tool is a development/deployment utility. It is **not** published to
Android; only the web app runs in Termux.

## Prerequisites

- .NET 10 SDK on the machine that can reach SQL Server.
- SQL Server reachable with either Windows authentication or a SQL login.
- The web app must build (`dotnet build`) so the SQLite migrations are present.

Run all commands from the repository root
(`AsliHalwaiWebApp/` — the folder containing `AsliHalwaiWebApp.sln`).

## 1. Build and check the plan (read-only)

```powershell
dotnet build tools/AsliHalwai.DbMigrator/AsliHalwai.DbMigrator.csproj

dotnet run --project tools/AsliHalwai.DbMigrator -- --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --dry-run
```

`--dry-run` reads the source schema and row counts but writes nothing.
If SQL authentication is used:

```powershell
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "Server=tcp:my-server,1433;Database=AsliHalwaiDb;User Id=sa;Password=CHANGE_ME;Encrypt=True;TrustServerCertificate=True" --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --dry-run
```

The printed source connection string is redacted.

## 2. Import into a new SQLite database

Stop the web app first. Make sure `AsliHalwaiWebApp/Data/AsliHalwaiDb.db` does
not exist (or is empty).

```powershell
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db"
```

The default target is `Data/AsliHalwaiDb.db`, but passing `--target`
explicitly is recommended so the path is unambiguous.

Expected ending:

```text
Foreign key check: OK
Integrity check : ok
SUCCESS: SQL Server data was imported into SQLite.
```

## 3. Verify an existing import

```powershell
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --verify-only
```

`--verify-only` does not write to either database. It compares all table
counts and runs SQLite foreign-key and integrity checks.

## 4. Re-import over an existing SQLite database

Only do this after taking your own backup. The tool also creates an online
backup in `Data/backups/` before changing the target.

```powershell
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "..." --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --replace-existing
```

Without `--yes`, the tool asks you to type `MIGRATE`. For unattended use:

```powershell
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "..." --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --replace-existing --yes
```

## Options

| Option | Meaning |
| --- | --- |
| `--source "<connection string>"` | SQL Server source. Required. Read-only. |
| `--target "<path or connection string>"` | SQLite target. Default: `Data/AsliHalwaiDb.db` relative to the current directory. |
| `--replace-existing` | Clear a non-empty target after an online backup. |
| `--yes`, `-y` | Skip the `MIGRATE` confirmation. |
| `--dry-run` | Print the plan and counts; write nothing. |
| `--verify-only` | Compare source/target counts and run SQLite checks. |
| `--verbose` | Print exception details on failure. |
| `--help`, `-h` | Show help. |

Exit codes: `0` success, `1` unexpected failure, `2` validation/verification
failure, `3` user aborted.

## Troubleshooting

- **`The server was not found or was not accessible`** — SQL Server is not
  running, TCP/IP is disabled, or the instance name is wrong. Try
  `Server=localhost\SQLEXPRESS` or `Server=.\SQLEXPRESS`.
- **Login failed** — use `Trusted_Connection=True` for Windows authentication,
  or a SQL login with `User Id` / `Password`. The tool does not change logins.
- **`Source column '...' is missing but the target column is NOT NULL`** — the
  SQL Server schema differs from the EF Core model. Do not bypass this; align
  the model/migration first.
- **`Refusing to modify a non-empty target`** — you pointed at a database that
  already has data. Back it up, then use `--replace-existing` if replacement is
  intended, or choose a new `--target`.
- **`The target SQLite database has pending migrations`** — run
  `dotnet ef database update` from `AsliHalwaiWebApp/` (back up first), or
  create a fresh target so the migrator can apply migrations itself.
- **Source row count changed while copying** — stop the web app writing to SQL
  Server and rerun.

## What is not copied

- SQL Server logins, users, permissions, jobs, and stored procedures — the app
  has none and SQLite does not use them.
- SQL Server backup history.
- Uploaded image files. They already live in
  `AsliHalwaiWebApp/wwwroot/uploads/` and are included in the publish output.
  Database rows store only their paths.

## After the import

Start the app with:

```text
ConnectionStrings__constr=Data Source=Data/AsliHalwaiDb.db;Cache=Shared;Foreign Keys=True
Database__AutoMigrate=false
```

The lookup-table seeder only runs when a lookup table is completely empty, so
imported rows are never overwritten.
