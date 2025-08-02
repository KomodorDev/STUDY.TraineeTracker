using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data;

/// <summary>
/// Represents the application's database context, integrating ASP.NET Identity and domain models.
/// </summary>
/// <remarks>
/// This context manages entities such as <see cref="ProcessingPause"/>, <see cref="TraineeLessonLogEntry"/>, <see cref="TraineeLesson"/>, 
/// <see cref="TraineeStatisticsSnapshot"/>, <see cref="Feedback"/>, <see cref="Lesson"/>, <see cref="TeachingPlan"/>, and <see cref="EmailNotificationSetting"/>.
/// It configures unique constraints, relationships, and enum-to-string conversions for the domain models.
/// Code Ownership: Simon Hinterreiter (hintsimo)
/// </remarks>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

    /// <summary>
    /// Gets or sets the collection of <see cref="ProcessingPause"/> entities in the database.
    /// </summary>
    public DbSet<ProcessingPause> ProcessingPauses { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="TraineeLessonLogEntry"/> entities in the database.
    /// Represents the log entries for lessons attended by trainees.
    /// </summary>
    public DbSet<TraineeLessonLogEntry> TraineeLessonLogEntries { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="TraineeLesson"/> entities in the database.
    /// </summary>
    public DbSet<TraineeLesson> TraineeLessons { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="TraineeStatisticsSnapshot"/> entities.
    /// Represents the snapshots of trainee statistics stored in the database.
    /// </summary>
    public DbSet<TraineeStatisticsSnapshot> TraineeStatisticsSnapshots { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="Feedback"/> entities in the database.
    /// </summary>
    public DbSet<Feedback> Feedbacks { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="Lesson"/> entities in the database.
    /// </summary>
    public DbSet<Lesson> Lessons { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="TeachingPlan"/> entities in the database.
    /// </summary>
    public DbSet<TeachingPlan> TeachingPlans { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="EmailNotificationSetting"/> entities in the database.
    /// </summary>
    public DbSet<EmailNotificationSetting> EmailNotificationSettings { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    // ------------------------------------------------------
    /// <summary>
    /// Configures the entity mappings and relationships for the application's database context.
    /// <para>
    /// The method performs the following configurations:
    /// <list type="bullet">
    ///   <item>
    ///     <description>Ensures that each <c>Lesson</c> has a unique combination of <c>MakandraId</c> and <c>TeachingPlanId</c>.</description>
    ///   </item>
    ///   <item>
    ///     <description>Enforces uniqueness of the <c>Name</c> property in <c>TeachingPlan</c> entities.</description>
    ///   </item>
    ///   <item>
    ///     <description>Configures a many-to-many relationship between <c>Feedback</c> and <c>ApplicationUser</c> via the <c>ReadByUsers</c> and <c>ReadFeedbacks</c> navigation properties, using a join table named "FeedbackRead".</description>
    ///   </item>
    ///   <item>
    ///     <description>Sets up a one-to-many relationship between <c>ApplicationUser</c> and <c>Feedback</c> for feedback authorship, requiring an <c>AuthorId</c> foreign key.</description>
    ///   </item>
    ///   <item>
    ///     <description>Maps the <c>State</c> enum property of <c>TraineeLesson</c> to its string representation in the database.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo)</remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // +++++++++++++++
        // Only one MakandraID per TeachingPlan
        modelBuilder.Entity<Lesson>()
            .HasIndex(l => new { l.MakandraId, l.TeachingPlanId })
            .IsUnique();

        // +++++++++++++++
        // TeachingPlan name is unique
        modelBuilder.Entity<TeachingPlan>()
            .HasIndex(tp => tp.Name)
            .IsUnique();

        // +++++++++++++++
        // ApplicationUser and Feedback
        modelBuilder.Entity<Feedback>()
            .HasMany(f => f.ReadByUsers)
            .WithMany(u => u.ReadFeedbacks)
            .UsingEntity<Dictionary<string, object>>(
                "FeedbackRead",
                j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("ReaderId"),
                j => j.HasOne<Feedback>().WithMany().HasForeignKey("FeedbackId"));

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Author)
            .WithMany(u => u.WrittenFeedbacks)
            .HasForeignKey(f => f.AuthorId)
            .IsRequired();

        // +++++++++++++++
        // Enum to String Mapping
        modelBuilder.Entity<TraineeLesson>()
            .Property(t => t.State)
            .HasConversion<string>();
    }

    // ------------------------------------------------------
}
