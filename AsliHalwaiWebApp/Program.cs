using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.CommonCode;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.Name = ".AsliHalwai.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddScoped<EmailSender>();

// Google authentication configuration is preserved from the original app.
// It is only registered when credentials are supplied, otherwise OAuth
// options validation would make every request fail.
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrWhiteSpace(googleClientId) &&
    !string.IsNullOrWhiteSpace(googleClientSecret))
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
        });
}

// Keep Data Protection keys next to the SQLite database so login/session
// cookies survive application restarts and phone reboots.
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
var keysDirectory = Path.Combine(dataDirectory, "keys");
Directory.CreateDirectory(keysDirectory);

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory))
    .SetApplicationName("AsliHalwaiWebApp");

// ================= DATABASE (SQLite) =================
var rawConnectionString = builder.Configuration.GetConnectionString("constr")
    ?? throw new InvalidOperationException(
        "Connection string 'constr' is missing. Set ConnectionStrings__constr.");

var sqliteConnectionString = SqliteDatabase.ResolveConnectionString(
    rawConnectionString,
    builder.Environment.ContentRootPath);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(sqliteConnectionString);
    options.AddInterceptors(new SqliteConnectionInterceptor());
});

var app = builder.Build();

// ================= DATABASE STARTUP =================
// Creates a missing database, applies migrations only when explicitly allowed,
// and always creates a backup before migrating an existing database.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DatabaseInitializer.Initialize(
        db,
        app.Configuration,
        app.Environment.ContentRootPath,
        app.Logger);
}

// ================= PIPELINE =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Session middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
