using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using TraineeTracker.Data;
using TraineeTracker.Models.Domain;
using TraineeTracker.Services.Seeders;
using TraineeTracker.Data.ApplicationUsers;

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

// Benötigte Repositories
builder.Services.AddScoped<IApplicationUserRepository, DatabaseApplicationUserRepository>();

// ----------------------------------------
// Register Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSender, GmailEmailSender>();

// ----------------------------------------



var app = builder.Build();

// Rollen erstellen, falls noch nicht in der Datenbank
using (var scope = app.Services.CreateScope()) {
    var serviceProvider = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAsync(serviceProvider);
    await IdentitySeeder.SeedTestUsersAsync(serviceProvider);
}

// ---------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var usersToSeed = new[]
    {
        new { Email = "simon.hinterreiter@uni-a.de", Role = "Admin" },
        new { Email = "paul.schweizer@uni-a.de", Role = "Mentor" },
        new { Email = "alexander.schlemmer@uni-a.de", Role = "Mentor" },
        new { Email = "alexandros.blask@uni-a.de", Role = "Trainee" },
        new { Email = "nikita.stefan@uni-a.de", Role = "Trainee" }
    };

    string password = "SoPro.2025";

    foreach (var entry in usersToSeed)
    {
        if (!await roleManager.RoleExistsAsync(entry.Role))
        {
            await roleManager.CreateAsync(new IdentityRole(entry.Role));
        }

        var user = await userManager.FindByEmailAsync(entry.Email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = entry.Email,
                Email = entry.Email,
                EmailConfirmed = true
                // EmailNotificationSettings = new EmailNotificationSettings() // ← vorerst auskommentiert
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                Console.WriteLine($"Fehler beim Erstellen von {entry.Email}:");
                foreach (var error in result.Errors)
                    Console.WriteLine($"- {error.Description}");
                continue;
            }
        }

        if (!await userManager.IsInRoleAsync(user, entry.Role))
        {
            await userManager.AddToRoleAsync(user, entry.Role);
        }
    }
}


// ---------------------------------------------
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
