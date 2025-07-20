using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public DbSet<ProcessingPause> ProcessingPauses { get; set; }

    public DbSet<TraineeLessonLogEntry> TraineeLessonLogEntries { get; set; }

    public DbSet<TraineeLesson> TraineeLessons { get; set; }

    public DbSet<TraineeStatisticsSnapshot> TraineeStatisticsSnapshots { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<TeachingPlan> TeachingPlans { get; set; }
    public DbSet<EmailNotificationSetting> EmailNotificationSettings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    // ------------------------------------------------------
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
            .WithMany(u => u.WrittenFeedbacks) // Stelle sicher, dass ApplicationUser diese Navigation hat
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
