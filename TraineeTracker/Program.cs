using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

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


// -----------------------------------------
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
// FeedbackService
// TraineeLessonDashboardService


// ----------------------------------------
// Register Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSender, GmailEmailSender>();

// ----------------------------------------



var app = builder.Build();

// ---------------------------------------------
// Create Roles if non-existent
using (var scope = app.Services.CreateScope()) {
    var serviceProvider = scope.ServiceProvider;
    var rolesSeeder = serviceProvider.GetRequiredService<RolesSeeder>();
    await rolesSeeder.SeedRolesAsync();

    // Testdaten (User, Lessons, TeachingPlans)
    var testDataSeeder = serviceProvider.GetRequiredService<TestDataSeeder>();
    await testDataSeeder.SeedUsersAsync();
    await testDataSeeder.SeedLessonsAsync();
    await testDataSeeder.SeedTeachingPlansAsync();
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
