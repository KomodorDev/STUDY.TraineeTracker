using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Data;
using TraineeTracker.Models.Domain;
using TraineeTracker.Services.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var environment = builder.Environment;  // NEU: Environment auslesen
Console.WriteLine($"🌍 Environment: {environment.EnvironmentName}");

if (environment.IsDevelopment()) {
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));  // InMemory für Tests
} else {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));  // Wie gehabt SQLite
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity konfigurieren
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Rollen erstellen, falls noch nicht in der Datenbank
using (var scope = app.Services.CreateScope()) {
    var serviceProvider = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAsync(serviceProvider);
    await IdentitySeeder.SeedTestUsersAsync(serviceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
