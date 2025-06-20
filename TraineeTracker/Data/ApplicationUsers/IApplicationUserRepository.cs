using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers
{
    public interface IApplicationUserRepository
    {
        bool Exists(ApplicationUser applicationUser);

        IEnumerable<ApplicationUser> GetAllTrainees();

        IEnumerable<ApplicationUser> GetAllMentors();

        IEnumerable<ApplicationUser> GetAllAdmins();
    }
}
