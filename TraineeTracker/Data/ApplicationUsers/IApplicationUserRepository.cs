using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers
{
    public interface IApplicationUserRepository
    {
        public Task<bool> ExistsAsync(int applicationUserId);

        public Task<bool> ExistsAsync(ApplicationUser applicationUser);

        public Task<IEnumerable<ApplicationUser>> GetAllTraineesAsync();

        public Task<IEnumerable<ApplicationUser>> GetAllMentorsAsync();

        public Task<IEnumerable<ApplicationUser>> GetAllAdminsAsync();
    }
}
