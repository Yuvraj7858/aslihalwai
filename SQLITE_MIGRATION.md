# SQLite migration, deployment, and operations

This document describes the SQL Server → SQLite migration for
**AsliHalwaiWebApp**, how to run it locally, how to deploy it to Android/ARM64
through Termux, and how to back up and restore the database.

Status: the application builds on .NET 10, the SQLite schema is created from EF
Core migrations, the existing SQL Server data has been imported into
`AsliHalwaiWebApp/Data/AsliHalwaiDb.db` and verified, and the app was smoke
tested against that imported data. The Termux/Android commands below could not
be executed in this Windows environment and must be run on the phone.

---

## 1. What changed

### Packages (`AsliHalwaiWebApp/AsliHalwaiWebApp.csproj`)

Removed:

- `Microsoft.EntityFrameworkCore` 8.0.0
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.0
- `Microsoft.EntityFrameworkCore.Tools` 8.0.0

Added / updated:

- Target framework `net10.0`
- `Microsoft.EntityFrameworkCore` 10.0.10
- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.10
- `Microsoft.EntityFrameworkCore.Design` 10.0.10 (for `dotnet ef`)
- `Microsoft.AspNetCore.Authentication.Google` 10.0.12 — keeps the Google
  authentication registration that existed in the original `Program.cs`
- `SQLitePCLRaw.bundle_e_sqlite3` 2.1.14 — explicitly pinned because the
  transitive 2.1.11 native package has advisory `GHSA-2m69-gcr7-jv3q`

`AsliHalwaiWebApp/.config/dotnet-tools.json` pins `dotnet-ef` 10.0.10.

### Configuration

- `AsliHalwaiWebApp/Program.cs` now uses `UseSqlite`, resolves the `Data
  Source` relative to the content root, persists Data Protection keys under
  `Data/keys`, registers the SQLite connection interceptor, and runs the
  startup database initializer.
- Google authentication is registered only when both
  `Authentication:Google:ClientId` and `Authentication:Google:ClientSecret`
  are configured. The original unconditional `AddGoogle` call caused a 500 on
  every request when those values were absent, because OAuth options
  validation rejects a null `ClientId`. With no credentials, the app starts
  normally and all non-Google features work unchanged.
- `AsliHalwaiWebApp/Data/AppDbContext.cs` applies `COLLATE NOCASE` to every
  string property so SQLite matches SQL Server's default case-insensitive
  behavior. No entities, relationships, or features were removed.
- `AsliHalwaiWebApp/Models/OrderTable.cs` and
  `TableReservationTable.cs` no longer use the SQL Server-only
  `[Column(TypeName = "datetime")]` mapping.
- `AsliHalwaiWebApp/appsettings.json`:
  - SQLite connection string
  - `Urls` = `http://127.0.0.1:5000`
  - `Database:AutoMigrate` = `false`
  - `Database:SeedReferenceData` = `true`
  - SMTP password removed
- `AsliHalwaiWebApp/Properties/launchSettings.json` uses port 5000.
- `.gitignore` excludes SQLite databases, WAL/SHM files, backups, Data
  Protection keys, local secrets, and publish output.
- `AsliHalwaiWebApp/Migrations/20261120081703_InitialSqlite.cs` is the new
  SQLite baseline migration. `Migrations/InitialSqlite.sql` is a generated
  reference script (SQLite does not support EF's idempotent script mode; use
  `dotnet ef database update` to apply migrations).

### New runtime files

| File | Purpose |
| --- | --- |
| `Data/SqliteDatabase.cs` | Resolves `Data Source` relative to the content root and creates directories. |
| `Data/SqliteConnectionInterceptor.cs` | Per-connection `busy_timeout=10000`, `foreign_keys=ON`, `synchronous=NORMAL`. |
| `Data/DatabaseInitializer.cs` | Creates a missing DB, applies migrations safely, enables WAL, backs up before migrating an existing DB. |
| `Data/ReferenceDataSeeder.cs` | Seeds lookup tables only when they are completely empty. |
| `tools/AsliHalwai.DbMigrator/` | One-time SQL Server → SQLite data copy/verify tool. See its README. |

### Startup database behavior

| Situation | Behavior |
| --- | --- |
| Database file missing | Created automatically, all migrations applied, empty lookup tables seeded. |
| Existing DB, no pending migrations | Startup continues normally. |
| Existing DB, pending migrations, `Database:AutoMigrate=false` | Warning logged; the database is **not changed**. |
| Existing DB, pending migrations, `Database:AutoMigrate=true` | An online SQLite backup is written to `Data/backups/`, then migrations run. |
| Lookup table already has data | Never overwritten by the seeder. |

The app never deletes or recreates the database during startup.

---

## 2. Run locally (Windows)

From the web project directory:

```powershell
cd D:\AsliHalwai\AsliHalwaiWebApp\AsliHalwaiWebApp\AsliHalwaiWebApp

dotnet tool restore
dotnet build
dotnet run
```

Open <http://127.0.0.1:5000>.

The default database is `Data/AsliHalwaiDb.db` relative to the content root.
If the file already exists it is used as-is. To use a different database for
testing, override the connection string:

```powershell
$env:ConnectionStrings__constr = "Data Source=Data/AsliHalwaiDb.test.db;Cache=Shared;Foreign Keys=True"
$env:Database__AutoMigrate = "false"
dotnet run
```

---

## 3. Migrate existing SQL Server data

The migrator is documented in detail in
[`tools/AsliHalwai.DbMigrator/README.md`](tools/AsliHalwai.DbMigrator/README.md).
Short version, from the repository root:

```powershell
# 1. Inspect the plan/counts; writes nothing
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --dry-run

# 2. Import into a new SQLite database
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db"

# 3. Re-verify later without writing anything
dotnet run --project tools/AsliHalwai.DbMigrator -- --source "Server=DESKTOP-KQ3J52R\SQLEXPRESS;Database=AsliHalwaiDb;Trusted_Connection=True;TrustServerCertificate=True" --target "AsliHalwaiWebApp/Data/AsliHalwaiDb.db" --verify-only
```

The tool opens SQL Server in a `RepeatableRead` transaction and only reads. It
refuses a non-empty SQLite target unless `--replace-existing` is given, and in
that case it creates an online backup first.

### Verified import result

The import was run against the existing local `SQLEXPRESS` database and all 29
EF-mapped tables matched exactly:

```text
Table                                 SQL Server       SQLite Status
AddressTypeTable                               4            4 OK
CartDealTable                                  0            0 OK
CartItemDetailTable                            5            5 OK
CartTable                                      2            2 OK
DiscountTable                                  0            0 OK
GenderTable                                    3            3 OK
OrderDealDetailTable                           0            0 OK
OrderItemDetailTable                          15           15 OK
OrderPaymentTable                              3            3 OK
OrderStatusTable                               5            5 OK
OrderTable                                    16           16 OK
OrderTypeTable                                 2            2 OK
ReservationStatusTable                         5            5 OK
StockDealDetailTable                           2            2 OK
StockDealTable                                 1            1 OK
StockItemCategoryTable                         4            4 OK
StockItemIngredientTable                       1            1 OK
StockItemReviewTable                           3            3 OK
StockItemTable                                12           12 OK
StockMenuCategoryTable                         4            4 OK
StockMenuItemTable                             0            0 OK
TableReservationTable                          2            2 OK
UserAddressTable                               7            7 OK
UserDetailTable                                5            5 OK
UserPasswordRecoveryTable                      6            6 OK
UserStatusTable                                5            5 OK
UserTable                                      8            8 OK
UserTypeTable                                  4            4 OK
VisibleStatusTable                             2            2 OK

Foreign key check: OK
Integrity check : ok
```

The original SQL Server database was not modified or deleted. The SQLite file
is `AsliHalwaiWebApp/Data/AsliHalwaiDb.db`; it is intentionally gitignored and
must be copied to the deployment target.

Additional SQLite checks:

```text
PRAGMA journal_mode;      -> wal
PRAGMA integrity_check;   -> ok
PRAGMA foreign_key_check; -> no rows
```

Uploaded images are already in `AsliHalwaiWebApp/wwwroot/uploads/`; the
database stores only paths, so include `wwwroot` in the publish output.

---

## 4. EF Core migrations going forward

Do **not** delete/recreate the database and do not use `EnsureDeleted`. Create a
new migration for each model change.

From `AsliHalwaiWebApp/`:

```powershell
dotnet tool restore

# Back up first (see section 6)
dotnet ef migrations add AddMyChange --project AsliHalwaiWebApp.csproj --startup-project AsliHalwaiWebApp.csproj

# Review the generated Up()/Down() before applying
dotnet ef database update --project AsliHalwaiWebApp.csproj --startup-project AsliHalwaiWebApp.csproj
```

For a production/phone database:

1. Stop the app.
2. Back up `Data/AsliHalwaiDb.db`.
3. Either run `dotnet ef database update` on the target, or start once with
   `Database__AutoMigrate=true` (this creates another backup).
4. Set `Database__AutoMigrate=false` again.

Never apply a destructive migration (dropping columns/tables or rewriting data)
without explicit approval. `InitialSqlite.sql` is a generated reference only;
SQLite does not support EF's `--idempotent` script option.

---

## 5. Android 16 ARM64 deployment with Termux

### Why `proot-distro` Ubuntu

Microsoft's official .NET Linux ARM64 runtime is built for glibc. Termux uses
Android's Bionic libc, so the official self-contained `linux-arm64` build
(including the bundled `libe_sqlite3.so`) is not guaranteed to start directly in
Termux. The supported route is to run the published app inside an Ubuntu
`proot-distro` rootfs managed by Termux. Cloudflare Tunnel can still run in
Termux and reach the app on `127.0.0.1:5000`.

### 5.1 Publish from Windows

From the repository root:

```powershell
dotnet publish AsliHalwaiWebApp\AsliHalwaiWebApp.csproj `
  -c Release -r linux-arm64 --self-contained true `
  -o publish\linux-arm64

New-Item -ItemType Directory -Force publish\linux-arm64\Data | Out-Null
Copy-Item AsliHalwaiWebApp\Data\AsliHalwaiDb.db publish\linux-arm64\Data\AsliHalwaiDb.db

Compress-Archive -Path publish\linux-arm64\* `
  -DestinationPath publish\aslihalwai-linux-arm64.zip -Force
```

The publish output contains the `AsliHalwaiWebApp` executable,
`libe_sqlite3.so`, `appsettings.json`, and `wwwroot` (including uploaded
images). It does **not** contain `Data/AsliHalwaiDb.db` unless you copy it as
shown above.

Transfer to the phone:

```powershell
adb push publish\aslihalwai-linux-arm64.zip /sdcard/Download/
```

Or copy the ZIP to the phone's Download folder manually.

### 5.2 Install the runtime environment in Termux

In Termux:

```bash
pkg update -y
pkg upgrade -y
pkg install -y proot-distro unzip
termux-setup-storage

proot-distro install ubuntu-24.04
proot-distro login ubuntu-24.04
```

Inside the Ubuntu shell:

```bash
apt update
apt install -y unzip ca-certificates libicu-dev sqlite3 tmux

mkdir -p ~/aslihalwai
cd ~/aslihalwai
unzip -o /sdcard/Download/aslihalwai-linux-arm64.zip
chmod +x AsliHalwaiWebApp
```

Create a start script so the environment is always identical:

```bash
cat > ~/aslihalwai/start.sh <<'EOF'
#!/usr/bin/env bash
cd "$(dirname "$0")"

# Optional root-only file with SMTP_PASSWORD=... (chmod 600, never commit).
if [ -f .env ]; then
  set -a
  . ./.env
  set +a
fi

export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://127.0.0.1:5000
export ConnectionStrings__constr="Data Source=Data/AsliHalwaiDb.db;Cache=Shared;Foreign Keys=True"
export Database__AutoMigrate=false
export Database__SeedReferenceData=true

# Never commit the SMTP password. Export it before running this script,
# or set it in a root-only file that is not in the repository.
if [ -n "${SMTP_PASSWORD:-}" ]; then
  export smtp__Password="$SMTP_PASSWORD"
fi

exec ./AsliHalwaiWebApp
EOF

chmod +x ~/aslihalwai/start.sh
```

Start it:

```bash
cd ~/aslihalwai
SMTP_PASSWORD='your-gmail-app-password' ./start.sh
```

In a second Termux session (or another Ubuntu shell), verify:

```bash
curl -I http://127.0.0.1:5000/
```

Use `tmux` to keep it running after closing the terminal:

```bash
cd ~/aslihalwai
tmux new -s aslihalwai
SMTP_PASSWORD='...' ./start.sh
# detach with Ctrl-B then D
# reattach with: tmux attach -t aslihalwai
```

### 5.3 Keep it alive across reboots

- Install **Termux:Boot** from F-Droid, open it once.
- In Termux: `termux-wake-lock`.
- Set Termux to "Unrestricted" battery usage in Android settings.
- Create `~/.termux/boot/aslihalwai.sh`:

```bash
#!/data/data/com.termux/files/usr/bin/sh
termux-wake-lock
nohup proot-distro login ubuntu-24.04 -- bash -lc '~/aslihalwai/start.sh' \
  >> ~/aslihalwai.log 2>&1 &
```

Make it executable:

```bash
chmod +x ~/.termux/boot/aslihalwai.sh
```

The database lives inside the Ubuntu rootfs at `~/aslihalwai/Data/AsliHalwaiDb.db`.
It survives app restarts, phone reboots, and app updates. It is deleted if
Termux/proot is uninstalled or Android app data is cleared, so keep off-device
backups.

For the boot script, put `SMTP_PASSWORD` in a root-only file (for example
`~/aslihalwai/.env`, mode `600`) and source it from `start.sh`; do not put the
password in the repository or in the boot script itself. If email is not
needed, the app runs normally without it (sending simply fails).

### 5.4 Cloudflare Tunnel

Quick tunnel (temporary random URL, good for testing):

```bash
pkg install -y cloudflared
cloudflared tunnel --url http://127.0.0.1:5000
```

Named tunnel (stable hostname):

```bash
cloudflared tunnel login
cloudflared tunnel create aslihalwai
cloudflared tunnel route dns aslihalwai asli.example.com
```

Create `~/.cloudflared/config.yml` using the tunnel UUID printed by
`cloudflared tunnel create` and the matching credentials file:

```yaml
tunnel: YOUR-TUNNEL-UUID
credentials-file: /data/data/com.termux/files/home/.cloudflared/YOUR-TUNNEL-UUID.json
ingress:
  - hostname: asli.example.com
    service: http://127.0.0.1:5000
  - service: http_status:404
```

Run it:

```bash
cloudflared tunnel run aslihalwai
```

The app listens only on `127.0.0.1:5000`; Cloudflare Tunnel is the public entry
point. Do not expose port 5000 directly to the internet. Keep Cloudflare
credentials outside the repository.

---

## 6. SQLite backup and restore

### Inside Termux/Ubuntu (recommended)

The `sqlite3` CLI is installed by the deployment steps above.

Back up while the app is running (uses SQLite's online backup API, including
committed WAL content):

```bash
cd ~/aslihalwai
mkdir -p Data/backups
sqlite3 Data/AsliHalwaiDb.db \
  ".backup 'Data/backups/AsliHalwaiDb_$(date +%Y%m%d_%H%M%S).db'"
```

Verify the backup:

```bash
sqlite3 Data/backups/AsliHalwaiDb_YYYYMMDD_HHMMSS.db \
  "PRAGMA integrity_check; PRAGMA foreign_key_check;"
```

Restore (stop the app first):

```bash
cd ~/aslihalwai
mkdir -p Data/backups
cp Data/AsliHalwaiDb.db Data/backups/pre_restore_$(date +%Y%m%d_%H%M%S).db
sqlite3 Data/AsliHalwaiDb.db \
  ".restore 'Data/backups/AsliHalwaiDb_YYYYMMDD_HHMMSS.db'"
sqlite3 Data/AsliHalwaiDb.db "PRAGMA integrity_check;"
```

Then start the app again.

### Windows

```powershell
cd D:\AsliHalwai\AsliHalwaiWebApp\AsliHalwaiWebApp\AsliHalwaiWebApp
New-Item -ItemType Directory -Force Data\backups | Out-Null

sqlite3.exe "Data\AsliHalwaiDb.db" ".backup 'Data\backups\AsliHalwaiDb_20261120_120000.db'"
```

Restore:

```powershell
sqlite3.exe "Data\AsliHalwaiDb.db" ".restore 'Data\backups\AsliHalwaiDb_20261120_120000.db'"
```

If `sqlite3.exe` is unavailable, stop the app and copy `AsliHalwaiDb.db`,
`AsliHalwaiDb.db-wal`, and `AsliHalwaiDb.db-shm` together; never copy only the
`.db` file while a WAL file exists.

The app also creates an automatic online backup in `Data/backups/` whenever it
is started with `Database__AutoMigrate=true` and pending migrations exist.

---

## 7. Secrets

`appsettings.json` no longer contains the SMTP password. Supply it at runtime:

```bash
export smtp__Password='your-gmail-app-password'
```

On Windows PowerShell:

```powershell
$env:smtp__Password = "your-gmail-app-password"
```

Google authentication is optional and stays disabled unless both values are
present at runtime:

```bash
export Authentication__Google__ClientId='...'
export Authentication__Google__ClientSecret='...'
```

On Windows PowerShell:

```powershell
$env:Authentication__Google__ClientId = "..."
$env:Authentication__Google__ClientSecret = "..."
```

Do not commit `appsettings.Production.json`, `.env` files, Cloudflare
credentials, or the database file. The old Gmail app password was previously
committed to git history; rotate it in the Google account and use a new app
password.

---

## 8. Verification performed

| Area | Result |
| --- | --- |
| Build (`dotnet build`, Release) | 0 errors (pre-existing nullable warnings remain) |
| Fresh SQLite DB creation + initial migration | Passed |
| Lookup seeding on empty DB | Passed; imported data untouched |
| Real SQL Server → SQLite import, 29 tables | All counts match; FK and integrity OK |
| App startup on `http://127.0.0.1:5000` | Passed in Development and Production |
| Startup without Google credentials | Passed (Google auth skipped); no `ClientId` 500 |
| Registration and login/session | Passed |
| Home, all items, menu/categories, product detail | Passed with imported products |
| Cart add/view and `GetCartCount` | Passed |
| COD order placement, order history, order details, invoice | Passed |
| UPI payment submission and admin Mark Paid | Passed |
| Admin order list/status transitions and invoice dashboard | Passed |
| Admin stock/settings/reservation pages | Passed |
| Table reservation creation and admin processing | Passed |
| Image upload and persisted path | Passed |
| 60 concurrent reads | 60/60 passed |
| 12 concurrent registration writes | 12/12 passed |
| 40 mixed read/write requests | 40/40 HTTP 200 |
| SQLite online backup + restore | Passed; marker removed and integrity OK |
| `linux-arm64` self-contained publish | Passed; `libe_sqlite3.so` included |
| Termux/Android execution | **Not run here** (no Android device); commands above must be validated on the phone |

Notes:

- SQLite is single-writer. WAL, a 10-second busy timeout, and NORMAL sync are
  enabled; this is sufficient for a single restaurant deployment. Very heavy
  concurrent writes may still return `SQLITE_BUSY`; `Cache=Shared` is retained
  as requested.
- `PRAGMA journal_mode=WAL` is persisted in the database file. Do not place the
  database on a network filesystem.
- `Cache=Shared` is kept exactly as required in the connection string. If
  write contention ever becomes a problem, removing `Cache=Shared` (keeping WAL
  and `Foreign Keys=True`) is the first tuning step.
- Data Protection keys are stored in `Data/keys/`. Losing them only invalidates
  existing login/session cookies; it does not affect database data.

---

## 9. Rollback

The original SQL Server database was not modified. To roll back the app:

1. Restore `Microsoft.EntityFrameworkCore.SqlServer` and the SQL Server
   connection string in `appsettings.json`.
2. Remove the SQLite provider packages.
3. Restore the previous `Program.cs` / `AppDbContext` changes.
4. Keep the SQLite database and backups; do not delete the SQL Server database.

If a future SQLite migration must be undone, restore the pre-migration backup
from `Data/backups/` instead of hand-editing the schema.

---

## 10. Quick reference

```text
Local run:        dotnet run                (http://127.0.0.1:5000)
Database:         AsliHalwaiWebApp/Data/AsliHalwaiDb.db
Backups:          AsliHalwaiWebApp/Data/backups/
Data protection:  AsliHalwaiWebApp/Data/keys/
Publish:          publish/linux-arm64/
Migrator:         tools/AsliHalwai.DbMigrator/
```

```text
ConnectionStrings__constr=Data Source=Data/AsliHalwaiDb.db;Cache=Shared;Foreign Keys=True
Database__AutoMigrate=false
Database__SeedReferenceData=true
smtp__Password=<set at runtime>
Authentication__Google__ClientId=<optional; Google auth off when unset>
Authentication__Google__ClientSecret=<optional; Google auth off when unset>
ASPNETCORE_URLS=http://127.0.0.1:5000
```
