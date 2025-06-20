using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;  // for [Required]

namespace TraineeTracker.Models.Domain
{
    public class TraineeLesson : IValidatableObject {

        [Required]
        private int _traineeLessonId { get; }

        [Required]
        private string _userId { get; }

        [Required]
        private TraineeLessonState _state { get; set; }

        private string? _rejectionReason { get; set; }

        private DateOnly? _dayStarted { get; set; }

        private DateOnly? _dayFinished { get; set; }

        [Required]
        private Lesson _lesson { get; } //set?

        public TraineeLesson(int traineeLessonId, string userId, Lesson lesson) {
            _traineeLessonId = traineeLessonId;
            _userId = userId;
            _state = /*openstate*/
            _lesson = lesson;
        }

        // rejectionReason not null or empty when state is rejected
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (_state == /*HIERMUSSRejectedSTATEREIN*/ && String.IsNullOrEmpty(_rejectionReason)) {
                yield return new ValidationResult(
                    "rejectionReason is required when state is \"Rejected\"",
                    new[] { nameof(_rejectionReason) }
                );
            }
        }
    }
}