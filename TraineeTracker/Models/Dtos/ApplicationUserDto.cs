using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    
    /// <summary>
    /// Data Transfer Object (DTO) representing an application user.
    /// Used for transferring user data between layers of the application.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class ApplicationUserDto {
        
        /// <summary>
        /// Gets or sets the email address of the user.
        /// Must be a valid email format and is required.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the password of the user.
        /// Must be at least 6 and at most 100 characters long.
        /// Required field and treated as a password data type.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "Sopro.2025";

        /// <summary>
        /// Gets or sets the role of the user (e.g., Admin, Trainee, Trainer).
        /// Required field.
        /// </summary>
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "";

        /// <summary>
        /// Gets or sets the ID of the associated teaching plan.
        /// Nullable; may not be set for all users.
        /// </summary>
        [Display(Name = "Teachingplan")]
        public int? TeachingPlanId { get; set; }

        /// <summary>
        /// Gets or sets the start date for a trainee.
        /// Only applicable for users with the Trainee role.
        /// Nullable; defaults to today's date.
        /// </summary>
        [Display(Name = "Start Date")]
        public DateOnly? TraineeStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        /// <summary>
        /// Gets or sets the end date for a trainee.
        /// Only applicable for users with the Trainee role.
        /// Nullable; defaults to today's date.
        /// </summary>
        [Display(Name = "End Date")]
        public DateOnly? TraineeEndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}