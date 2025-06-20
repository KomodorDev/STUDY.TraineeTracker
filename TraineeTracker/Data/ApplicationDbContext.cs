using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Data;

using TraineeTracker.Models.Domain; // for TraineeLesson

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<TraineeLesson> TraineeLessons { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }
}
