using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Data;

public class ApplicationDbContext : IdentityDbContext {


    // Lists
    /*
    public DbSet<TraineeLesson> TraineeLessons { get; set; }
    */

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
