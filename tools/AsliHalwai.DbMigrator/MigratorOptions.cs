namespace AsliHalwai.DbMigrator;

public sealed class MigratorOptions
{
    public string? Source { get; private set; }
    public string Target { get; private set; } = "Data/AsliHalwaiDb.db";
    public bool ReplaceExisting { get; private set; }
    public bool Yes { get; private set; }
    public bool DryRun { get; private set; }
    public bool VerifyOnly { get; private set; }
    public bool Verbose { get; private set; }
    public bool ShowHelp { get; private set; }

    public static MigratorOptions Parse(string[] args)
    {
        var options = new MigratorOptions();

        for (var i = 0; i < args.Length; i++)
        {
            var raw = args[i];
            var argument = raw;
            string? inlineValue = null;

            var equalsIndex = raw.IndexOf('=');
            if (equalsIndex > 0 && raw.StartsWith("--", StringComparison.Ordinal))
            {
                argument = raw[..equalsIndex];
                inlineValue = raw[(equalsIndex + 1)..];
            }

            string NextValue(string name)
            {
                if (inlineValue is not null)
                    return inlineValue;

                if (i + 1 >= args.Length)
                    throw new ArgumentException($"{name} requires a value.");

                return args[++i];
            }

            switch (argument.ToLowerInvariant())
            {
                case "--source":
                    options.Source = NextValue("--source");
                    break;
                case "--target":
                    options.Target = NextValue("--target");
                    break;
                case "--replace-existing":
                    options.ReplaceExisting = true;
                    break;
                case "--yes":
                case "-y":
                    options.Yes = true;
                    break;
                case "--dry-run":
                    options.DryRun = true;
                    break;
                case "--verify-only":
                    options.VerifyOnly = true;
                    break;
                case "--verbose":
                    options.Verbose = true;
                    break;
                case "--help":
                case "-h":
                case "/?":
                    options.ShowHelp = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {raw}");
            }
        }

        if (!options.ShowHelp)
        {
            if (string.IsNullOrWhiteSpace(options.Source))
                throw new ArgumentException("--source is required.");

            if (options.DryRun && options.VerifyOnly)
                throw new ArgumentException("--dry-run and --verify-only cannot be combined.");

            if (options.VerifyOnly && options.ReplaceExisting)
                throw new ArgumentException("--replace-existing cannot be used with --verify-only.");
        }

        return options;
    }

    public static void PrintHelp()
    {
        Console.WriteLine(
"""
AsliHalwai SQL Server -> SQLite data migrator

The tool NEVER writes to the SQL Server database. It reads the source,
creates/migrates the SQLite schema, copies every EF Core mapped table,
checks foreign keys and compares record counts.

Usage:
  AsliHalwai.DbMigrator --source "<SQL Server connection string>" [options]

Required:
  --source "<connection string>"   SQL Server source (read-only usage).

Options:
  --target "<path|connection>"     SQLite target. Default: Data/AsliHalwaiDb.db
  --replace-existing               Clear a non-empty target after backing it up.
                                   Requires interactive confirmation unless --yes.
  --yes, -y                        Skip the replace confirmation prompt.
  --dry-run                        Validate source/target plan, write nothing.
  --verify-only                    Only compare source and target counts/FKs.
  --verbose                        Print exception details.
  --help, -h                       Show this help.

Examples:
  # Validate without writing
  dotnet run --project tools/AsliHalwai.DbMigrator -- \
    --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" \
    --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" \
    --dry-run

  # Import into a new SQLite database
  dotnet run --project tools/AsliHalwai.DbMigrator -- \
    --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" \
    --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db"

  # Re-import into an existing SQLite database (backup is created first)
  dotnet run --project tools/AsliHalwai.DbMigrator -- \
    --source "..." --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" \
    --replace-existing --yes

  # Verify an existing import
  dotnet run --project tools/AsliHalwai.DbMigrator -- \
    --source "..." --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --verify-only
""");
    }
}
