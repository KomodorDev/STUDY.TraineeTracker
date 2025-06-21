using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ProcessingPause> ProcessingPauses { get; set; }
    
    public DbSet<TraineeLessonLogEntry> TraineeLessonLogEntries { get; set; }
    
    public DbSet<TraineeLesson> TraineeLessons {
        get; set;
    }
    
    public DbSet<TraineeStatisticsSnapshot> TraineeStatisticsSnapshots {
        get; set;
    }

    public DbSet<Feedback> Feedbacks {
        get; set;
    }
    public DbSet<Lesson> Lessons {
        get; set;
    }
    public DbSet<TeachingPlan> TeachingPlans {
        get; set;
    }
    public DbSet<EmailNotificationSetting> EmailNotificationSettings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    /*
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enum to String Mapping
        modelBuilder.Entity<TraineeLesson>()
            .Property(t => t.State)
            .HasConversion<string>();
    }
    */

}
