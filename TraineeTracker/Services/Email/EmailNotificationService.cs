using TraineeTracker.Models.Domain;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models.Dtos;
using Microsoft.AspNetCore.Identity.UI.Services;
using TraineeTracker.Data.EmailNotificationSettings;

namespace TraineeTracker.Services.Email {
    public class EmailNotificationService {
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;
        private readonly IEmailSender _emailSender;
        private readonly IEmailNotificationSettingRepository _databaseEmailNotificationSettingRepository;

        // ------------------------------------------------------
        public EmailNotificationService(IEmailSender emailSender, IApplicationUserRepository databaseApplicationUserRepository, IEmailNotificationSettingRepository databaseEmailNotificationSettingRepository) {
            _emailSender = emailSender;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _databaseEmailNotificationSettingRepository = databaseEmailNotificationSettingRepository;
        }

        // ------------------------------------------------------
        public async Task SaveNotificationSettingChange(string userId, NotificationSettingDto update) {

            // Load setting
            var setting = await _databaseEmailNotificationSettingRepository.GetByUserIdAsync(userId);

            // Update Setting
            setting.ReceiveSkippedNotifications = update.ReceiveSkippedNotifications;
            setting.ReceiveOpenNotifications = update.ReceiveOpenNotifications;
            setting.ReceiveStartedNotifications = update.ReceiveStartedNotifications;
            setting.ReceiveFinishedNotifications = update.ReceiveFinishedNotifications;
            setting.ReceiveRejectedNotifications = update.ReceiveRejectedNotifications;
            setting.ReceiveAcceptedNotifications = update.ReceiveAcceptedNotifications;
            setting.ReceiveRatedNotifications = update.ReceiveRatedNotifications;

            setting.ReceiveImportChangeNotifications = update.ReceiveImportChangeNotifications;

            // Store Setting
            await _databaseEmailNotificationSettingRepository.UpdateAsync(setting);
        }

        // ------------------------------------------------------
        public async Task<NotificationSettingDto> GetNotificationSetting(string userId) {
            var setting = await _databaseEmailNotificationSettingRepository.GetByUserIdAsync(userId);

            return new NotificationSettingDto {
                ReceiveSkippedNotifications = setting.ReceiveSkippedNotifications,
                ReceiveOpenNotifications = setting.ReceiveOpenNotifications,
                ReceiveStartedNotifications = setting.ReceiveStartedNotifications,
                ReceiveFinishedNotifications = setting.ReceiveFinishedNotifications,
                ReceiveRejectedNotifications = setting.ReceiveRejectedNotifications,
                ReceiveAcceptedNotifications = setting.ReceiveAcceptedNotifications,
                ReceiveRatedNotifications = setting.ReceiveRatedNotifications,
                ReceiveImportChangeNotifications = setting.ReceiveImportChangeNotifications
            };
        }

        // ------------------------------------------------------
        public async Task NotifyUserAsync(ApplicationUser user, string subject, string message) {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("E-Mail-Address is not set");

            await _emailSender.SendEmailAsync(
                user.Email,
                subject,
                $"<p>{message}</p>");
        }

        // ------------------------------------------------------
        private bool ShouldNotify(EmailNotificationSetting setting, TraineeLessonState newState) {
            return (newState == TraineeLessonState.Skipped && setting.ReceiveSkippedNotifications) ||
                (newState == TraineeLessonState.Open && setting.ReceiveOpenNotifications) ||
                (newState == TraineeLessonState.Started && setting.ReceiveStartedNotifications) ||
                (newState == TraineeLessonState.Finished && setting.ReceiveFinishedNotifications) ||
                (newState == TraineeLessonState.Rejected && setting.ReceiveRejectedNotifications) ||
                (newState == TraineeLessonState.Accepted && setting.ReceiveAcceptedNotifications) ||
                (newState == TraineeLessonState.Rated && setting.ReceiveRatedNotifications);
        }


        // ------------------------------------------------------
        public async Task NotifyAboutStateChangeAsync(TraineeLesson traineeLesson, TraineeLessonState oldState, TraineeLessonState newState) {

            // Get Trainee
            var trainee = await _databaseApplicationUserRepository.FindByIdWithNotificationSettingAsync(traineeLesson.TraineeId);

            var traineeSetting = trainee!.EmailNotificationSetting;
            string traineeName = trainee!.UserName!;
            string lessonTitle = traineeLesson.Lesson.Title;

            // Rejected Note:
            string rejectedReason = traineeLesson.RejectionReason!;
            var rejectionNote = "";
            if (newState == TraineeLessonState.Rejected && !string.IsNullOrWhiteSpace(traineeLesson.RejectionReason)) {
                rejectionNote = $"<p><strong>Rejection Reason:</strong> {rejectedReason}</p>";
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // Notify Mentors and Admins
            var mentors = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Mentor");
            var admins = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Admin");

            // Concat
            var thirdPersons = mentors
                .Concat(admins)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var person in thirdPersons) {
                var setting = person.EmailNotificationSetting!;

                if (person.IsClosed || !ShouldNotify(setting, newState))
                    continue;

                var subject = $"TraineeTracker: Trainee '{traineeName}': Lesson '{lessonTitle}' changed from {oldState} to {newState}";


                var messageHtml = $@"
                    <p>State Change:</p>
                    <p>The lesson <strong>“{lessonTitle}”</strong> of trainee <strong>{traineeName}</strong> has changed.</p>
                    <p><strong>Previous:</strong> {oldState}<br/>
                    <strong>New:</strong> {newState}</p>
                    {rejectionNote}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(person.Email!, subject, messageHtml);

                /* 
                Console.WriteLine("Sending email to: " + person.Email);
                Console.WriteLine("Subject: " + subject);
                 */
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // Notifiy Trainee
            if (!trainee.IsClosed && ShouldNotify(traineeSetting, newState)) {
                var subject = $"TraineeTracker: Lesson '{lessonTitle}' changed from {oldState} to {newState}";

                var messageHtml = $@"
                    <p>State Change:</p>
                    <p>The state of your lesson <strong>“{lessonTitle}”</strong> has changed.</p>
                    <p><strong>Previous:</strong> {oldState}<br/>
                    <strong>New:</strong> {newState}</p>
                    {rejectionNote}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(trainee.Email!, subject, messageHtml);

                /* 
                Console.WriteLine("Sending email to: " + trainee.Email);
                Console.WriteLine("Subject: " + subject);
                 */
            }

        }

        // ------------------------------------------------------
        public async Task NotifyAboutImportChangeAsync(ApplicationUser trainee, List<TraineeLesson> removedLessons, List<TraineeLesson> addedLessons) {
            var mentors = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Mentor");
            var admins = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Admin");

            var thirdPersons = mentors
                .Concat(admins)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            var added = addedLessons.Select(l => l.Lesson.Title).ToList();
            var removed = removedLessons.Select(l => l.Lesson.Title).ToList();

            // ++++++++++++++++++++++++++++++++++++++++++
            // Notifiy Trainee
            var setting = await _databaseEmailNotificationSettingRepository.GetByUserIdAsync(trainee.Id);
            if (setting.ReceiveImportChangeNotifications && (added.Any() || removed.Any())) {
                var subject = "TraineeTracker: Your teaching plan has been updated";

                var changes = "";

                if (added.Any()) {
                    changes += "<p><strong>New lessons assigned:</strong><br/>" +
                            string.Join("<br/>", added.Select(n => $"– {n}")) + "</p>";
                }

                if (removed.Any()) {
                    changes += "<p><strong>Lessons removed:</strong><br/>" +
                            string.Join("<br/>", removed.Select(n => $"– {n}")) + "</p>";
                }

                var messageHtml = $@"
                    <p>Hello {trainee.UserName},</p>
                    <p>Your teaching plan has been updated. Here is a summary of the changes:</p>
                    {changes}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(trainee.Email!, subject, messageHtml);
            }


            // ++++++++++++++++++++++++++++++++++++++++++
            // Notify Mentors and Admins
            if (added.Any() || removed.Any()) {
                var subject = $"TraineeTracker: Changes to {trainee.UserName}'s TeachingPlan";

                var messageHtml = $@"
                    <p>The following changes were made during a teaching plan import for trainee <strong>{trainee.UserName}</strong>:</p>
                    <ul>
                        <li><strong>Added Lessons:</strong> {added.Count}</li>
                        <li><strong>Removed Lessons:</strong> {removed.Count}</li>
                    </ul>
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                foreach (var person in thirdPersons) {
                    var s = person.EmailNotificationSetting!;
                    if (person.IsClosed || !s.ReceiveImportChangeNotifications)
                        continue;

                    await _emailSender.SendEmailAsync(person.Email!, subject, messageHtml);
                }
            }
        }


        // ------------------------------------------------------
        public EmailNotificationSetting CreateDefaultEmailNotificationSetting(string role) {
            var setting = new EmailNotificationSetting();

            switch (role) {
                case "Trainee":
                    setting.ReceiveSkippedNotifications = true;
                    setting.ReceiveOpenNotifications = false;
                    setting.ReceiveStartedNotifications = false;
                    setting.ReceiveFinishedNotifications = false;
                    setting.ReceiveRejectedNotifications = true;
                    setting.ReceiveAcceptedNotifications = true;
                    setting.ReceiveRatedNotifications = false;
                    setting.ReceiveImportChangeNotifications = true;
                    break;

                case "Mentor":
                case "Admin":
                    setting.ReceiveSkippedNotifications = false;
                    setting.ReceiveOpenNotifications = false;
                    setting.ReceiveStartedNotifications = false;
                    setting.ReceiveFinishedNotifications = true;
                    setting.ReceiveRejectedNotifications = false;
                    setting.ReceiveAcceptedNotifications = false;
                    setting.ReceiveRatedNotifications = true;
                    setting.ReceiveImportChangeNotifications = true;
                    break;

                default:
                    throw new ArgumentException($"Unknown role '{role}' for default settings.");
            }

            return setting;
        }

        // ------------------------------------------------------

    }
}
