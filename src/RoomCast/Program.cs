using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RoomCast.Data;
using RoomCast.Models;
using RoomCast.Services.MediaPreview;

var builder = WebApplication.CreateBuilder(args);

// FORCE DEVELOPMENT
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

// FORCE PORT 7058
builder.WebHost.UseUrls("http://0.0.0.0:7058");


// FIX DB PATH (THIS IS YOUR REAL DB)
var dbPath = @"C:\Users\ace_i\OneDrive\Desktop\RoomCast\src\RoomCast\app.db";

// REGISTER EF + SQLITE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

// MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IMediaFilePreviewBuilder, MediaFilePreviewBuilder>();

var app = builder.Build();

// AUTO CREATE DB & MIGRATE
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// STATIC FILES
app.UseStaticFiles();

// 🔥 FIX UPLOADS FOLDER WHEN RUNNING FROM PUBLISH
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// 🔥 FIX SCREEN DISPLAY ROUTE (IMPORTANT)
app.MapControllerRoute(
    name: "screen-display",
    pattern: "Screens/ScreenDisplay/{screenId:guid}",
    defaults: new { controller = "Screens", action = "ScreenDisplay" }
);

// DEFAULT ROUTE
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
