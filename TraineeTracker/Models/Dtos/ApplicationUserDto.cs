using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class ApplicationUserDto {
        public required String Email { get; set; }

        public required String Password { get; set; }
        
        public required String Role { get; set; }
    }
}