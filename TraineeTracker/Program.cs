using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System.Globalization;

using TraineeTracker.Models.Domain;

using TraineeTracker.Services;
using TraineeTracker.Services.Seeders;
using TraineeTracker.Services.Admin;
using TraineeTracker.Services.Email;

using TraineeTracker.Data;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.EmailNotificationSettings;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Data.UnitOfWork;

// -----------------------------------------
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------
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

// -----------------------------------------
// Identity konfigurieren
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


// -----------------------------------------

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Repositories
builder.Services.AddScoped<IApplicationUserRepository, DatabaseApplicationUserRepository>();
builder.Services.AddScoped<IEmailNotificationSettingRepository, DatabaseEmailNotificationSettingRepository>();
builder.Services.AddScoped<IFeedbackRepository, DatabaseFeedbackRepository>();
builder.Services.AddScoped<ILessonRepository, DatabaseLessonRepository>();
builder.Services.AddScoped<IProcessingPauseRepository, DatabaseProcessingPauseRepository>();
builder.Services.AddScoped<ITeachingPlanRepository, DatabaseTeachingPlanRepository>();
builder.Services.AddScoped<ITraineeLessonLogEntryRepository, DatabaseTraineeLessonLogEntryRepository>();
builder.Services.AddScoped<ITraineeLessonRepository, DatabaseTraineeLessonRepository>();
builder.Services.AddScoped<ITraineeStatisticsRepository, DatabaseTraineeStatisticsRepository>();

// -----------------------------------------
// HttpClient
builder.Services.AddHttpClient();


// -----------------------------------------
// Services
builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<AdminService>();

builder.Services.AddScoped<TeachingPlanService>();
builder.Services.AddScoped<TraineeLessonDetailService>();
builder.Services.AddScoped<TraineeStatisticsService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<TraineeLessonDashboardService>();

// ----------------------------------------
// Register Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSender, GmailEmailSender>();

// -----------------------------------------
// Seeder Services
builder.Services.AddScoped<RolesSeeder>();
builder.Services.AddScoped<TestDataSeeder>();

// ----------------------------------------
// Set culture settings
var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

builder.Services.Configure<RequestLocalizationOptions>(options => {
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = new[] { defaultCulture };
    options.SupportedUICultures = new[] { defaultCulture };
});

// ----------------------------------------
// Build
var app = builder.Build();

// ----------------------------------------
// Apply culture settings
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

// ----------------------------------------
// Ensure DB is there:
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var database = db.Database;

    if (database.IsRelational()) {
        database.Migrate();
    }
}

// ---------------------------------------------
// Seeding
using (var scope = app.Services.CreateScope()) {
    var serviceProvider = scope.ServiceProvider;
    var rolesSeeder = serviceProvider.GetRequiredService<RolesSeeder>();
    await rolesSeeder.SeedRolesAsync();

    // Testdaten (User, Lessons, TeachingPlans)
    var testDataSeeder = serviceProvider.GetRequiredService<TestDataSeeder>();
    await testDataSeeder.SeedTeachingPlansAsync();
    await testDataSeeder.SeedLessonsAsync();
    await testDataSeeder.SeedUsersAsync();
    await testDataSeeder.SeedProcessingPausesAsync();
    await testDataSeeder.SeedFeedbackAsync();
    /*     await testDataSeeder.SeedTraineeStatisticsSnapshotAsync(); */
    await testDataSeeder.SeedProgressForStefanAndUrsulaAsync();
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

// set dashboard as standard page <do not use yet>
app.MapGet("/", context => {
    context.Response.Redirect("/Dashboard");
    return Task.CompletedTask;
});


app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
