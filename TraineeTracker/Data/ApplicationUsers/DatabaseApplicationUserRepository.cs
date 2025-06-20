using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers
{
    public class DatabaseApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseApplicationUserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public bool Exists(ApplicationUser applicationUser)
        {
            return _userManager.Users.Any(u => u.Id == applicationUser.Id);
        }

        public IEnumerable<ApplicationUser> GetAllTrainees()
        {
            return _userManager.GetUsersInRoleAsync("Trainee").Result;
        }

        public IEnumerable<ApplicationUser> GetAllMentors()
        {
            return _userManager.GetUsersInRoleAsync("Mentor").Result;
        }

        public IEnumerable<ApplicationUser> GetAllAdmins()
        {
            return _userManager.GetUsersInRoleAsync("Admin").Result;
        }
    }
}
