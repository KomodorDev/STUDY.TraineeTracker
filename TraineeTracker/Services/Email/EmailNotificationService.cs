using TraineeTracker.Models.Domain;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models.Dtos;
using Microsoft.AspNetCore.Identity.UI.Services;
using TraineeTracker.Data;

namespace TraineeTracker.Services {
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
        public async Task SaveNotificationSettingsChange(string userId, NotificationSettingDto update) {

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

            var trainee = traineeLesson.Trainee;
            var traineeSetting = trainee.EmailNotificationSetting!;
            string traineeName = trainee.UserName!;
            string lessonTitle = traineeLesson.Lesson.Title;

            // ++++++++++++++++++++++++++++++++++++++++++
            // Notify Mentors and Admins
            var mentors = await _databaseApplicationUserRepository.GetUsersInRoleAsync("Mentor");
            var admins = await _databaseApplicationUserRepository.GetUsersInRoleAsync("Admin");

            // Concat
            var thirdPersons = mentors
                .Concat(admins)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var person in thirdPersons) {
                var setting = person.EmailNotificationSetting!;

                if (!ShouldNotify(setting, newState))
                    continue;

                var subject = $"TraineeTracker: Trainee '{traineeName}': Lesson '{lessonTitle}' changed from {oldState} to {newState}";


                var messageHtml = $@"
                    <p>Status update:</p>
                    <p>The lesson <strong>“{lessonTitle}”</strong> of trainee <strong>{traineeName}</strong> has changed.</p>
                    <p><strong>Previous:</strong> {oldState}<br/>
                    <strong>New:</strong> {newState}</p>
                    <p>– TraineeTracker Notification System</p>";

                await _emailSender.SendEmailAsync(person.Email!, subject, messageHtml);
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // Notifiy Trainee
            if (ShouldNotify(traineeSetting, newState)) {
                var subject = $"TraineeTracker: Lesson '{lessonTitle}' changed from {oldState} to {newState}";

                var messageHtml = $@"
                    <p>Hello {trainee.UserName},</p>
                    <p>The status of your lesson <strong>“{lessonTitle}”</strong> has changed.</p>
                    <p><strong>Previous:</strong> {oldState}<br/>
                    <strong>New:</strong> {newState}</p>
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(trainee.Email!, subject, messageHtml);
            }

        }

        // ------------------------------------------------------
        public async Task NotifyAboutImportChangeAsync(ApplicationUser trainee, List<TraineeLesson> removedLessons, List<TraineeLesson> addedLessons) {
            var mentors = await _databaseApplicationUserRepository.GetUsersInRoleAsync("Mentor");
            var admins = await _databaseApplicationUserRepository.GetUsersInRoleAsync("Admin");

            var thirdPersons = mentors
                .Concat(admins)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            var added = addedLessons.Select(l => l.Lesson.Title).ToList();
            var removed = removedLessons.Select(l => l.Lesson.Title).ToList();

            // ++++++++++++++++++++++++++++++++++++++++++
            // Notifiy Trainee
            var setting = trainee.EmailNotificationSetting!;
            if (setting.ReceiveImportChangeNotifications && (added.Any() || removed.Any())) {
                var subject = "TraineeTracker: Your lesson plan has been updated";

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
                    <p>Your lesson plan has been updated. Here is a summary of the changes:</p>
                    {changes}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(trainee.Email!, subject, messageHtml);
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // Notify Mentors and Admins
            if (added.Any() || removed.Any()) {
                var subject = $"TraineeTracker: Changes to {trainee.UserName}'s TeachingPlan";

                var messageHtml = $@"
                    <p>The following changes were made during a TeachingPlan import for trainee <strong>{trainee.UserName}</strong>:</p>
                    <ul>
                        <li><strong>Added Lessons:</strong> {added.Count}</li>
                        <li><strong>Removed Lessons:</strong> {removed.Count}</li>
                    </ul>
                    <p>– TraineeTracker Notification System</p>";

                foreach (var person in thirdPersons) {
                    var s = person.EmailNotificationSetting!;
                    if (!s.ReceiveImportChangeNotifications)
                        continue;

                    await _emailSender.SendEmailAsync(person.Email!, subject, messageHtml);
                }
            }
        }


    }
}
