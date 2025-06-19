using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data;

public class ApplicationDbContext : IdentityDbContext {

    public DbSet<EmailNotificationSettings> EmailNotificationSettings {
        get; set;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }
}
