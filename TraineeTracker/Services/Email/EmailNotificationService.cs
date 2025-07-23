using TraineeTracker.Models.Domain;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models.Dtos;
using Microsoft.AspNetCore.Identity.UI.Services;
using TraineeTracker.Data.EmailNotificationSettings;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Extensions;

namespace TraineeTracker.Services.Email {

    /// <summary>
    /// Provides functionality for sending email notifications based on trainee lesson state changes 
    /// and teaching plan imports. Handles individual notification preferences for each user role.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class EmailNotificationService {

        /// <summary>
        /// Repository for retrieving and updating application user data, including roles and email addresses.
        /// </summary>
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;

        /// <summary>
        /// Service for sending emails asynchronously to users.
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Repository for retrieving and persisting user-specific email notification settings.
        /// </summary>
        private readonly IEmailNotificationSettingRepository _databaseEmailNotificationSettingRepository;

        /// <summary>
        /// Repository for retrieving lesson specific data
        /// </summary>
        private readonly ILessonRepository _databaseLessonRepository;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotificationService"/> class.
        /// </summary>
        /// <param name="emailSender">Service used to send emails.</param>
        /// <param name="databaseApplicationUserRepository">Repository to access application user data.</param>
        /// <param name="databaseEmailNotificationSettingRepository">Repository to access and update email notification settings.</param>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public EmailNotificationService(IEmailSender emailSender, IApplicationUserRepository databaseApplicationUserRepository, IEmailNotificationSettingRepository databaseEmailNotificationSettingRepository, ILessonRepository databaseLessonRepository) {
            _emailSender = emailSender;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _databaseEmailNotificationSettingRepository = databaseEmailNotificationSettingRepository;
            _databaseLessonRepository = databaseLessonRepository;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates a user's email notification settings based on the provided data transfer object (DTO).
        /// </summary>
        /// <param name="userId">The ID of the user whose settings should be updated.</param>
        /// <param name="update">The updated notification settings.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task SaveNotificationSettingChange(string userId, EmailNotificationSettingDto update) {

            // 1. Load setting
            var setting = await _databaseEmailNotificationSettingRepository.GetByUserIdAsync(userId);

            // 2. Update Setting
            setting.ReceiveSkippedNotifications = update.ReceiveSkippedNotifications;
            setting.ReceiveOpenNotifications = update.ReceiveOpenNotifications;
            setting.ReceiveStartedNotifications = update.ReceiveStartedNotifications;
            setting.ReceiveFinishedNotifications = update.ReceiveFinishedNotifications;
            setting.ReceiveRejectedNotifications = update.ReceiveRejectedNotifications;
            setting.ReceiveAcceptedNotifications = update.ReceiveAcceptedNotifications;
            setting.ReceiveRatedNotifications = update.ReceiveRatedNotifications;

            setting.ReceiveImportChangeNotifications = update.ReceiveImportChangeNotifications;
            setting.ReceiveFeedbackChangeNotifications = update.ReceiveFeedbackChangeNotifications;

            // 3. Store Setting
            await _databaseEmailNotificationSettingRepository.UpdateAsync(setting);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the email notification settings for the specified user as a data transfer object (DTO).
        /// </summary>
        /// <param name="userId">The ID of the user whose notification settings should be retrieved.</param>
        /// <returns>
        /// A Task that represents the asynchronous operation. The task result contains a <see cref="NotificationSettingDto"/>
        /// with the user's current email notification preferences.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task<EmailNotificationSettingDto> GetNotificationSetting(string userId) {
            var setting = await _databaseEmailNotificationSettingRepository.GetByUserIdAsync(userId);

            return new EmailNotificationSettingDto {
                ReceiveSkippedNotifications = setting.ReceiveSkippedNotifications,
                ReceiveOpenNotifications = setting.ReceiveOpenNotifications,
                ReceiveStartedNotifications = setting.ReceiveStartedNotifications,
                ReceiveFinishedNotifications = setting.ReceiveFinishedNotifications,
                ReceiveRejectedNotifications = setting.ReceiveRejectedNotifications,
                ReceiveAcceptedNotifications = setting.ReceiveAcceptedNotifications,
                ReceiveRatedNotifications = setting.ReceiveRatedNotifications,
                ReceiveImportChangeNotifications = setting.ReceiveImportChangeNotifications,
                ReceiveFeedbackChangeNotifications = setting.ReceiveFeedbackChangeNotifications
            };
        }

        // ------------------------------------------------------
        /// <summary>
        /// Sends an email notification to a specified user.
        /// Throws an InvalidOperationException if the user's email address is missing.
        /// </summary>
        /// <param name="user">The recipient of the email. Must have a valid email address set.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="message">The HTML message body to be included in the email.</param>
        /// <exception cref="InvalidOperationException">Thrown if the user's email is null or empty.</exception>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task NotifyUserAsync(ApplicationUser user, string subject, string message) {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("E-Mail-Address is not set");

            await _emailSender.SendEmailAsync(
                user.Email,
                subject,
                $"<p>{message}</p>");
        }

        // ------------------------------------------------------
        /// <summary>
        /// Determines whether a user should be notified based on their email notification settings
        /// and the new state of a lesson.
        /// </summary>
        /// <param name="setting">The user's configured email notification preferences.</param>
        /// <param name="newState">The new state of the lesson that may trigger a notification.</param>
        /// <returns>
        /// True if the user should be notified for the given state, otherwise false.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
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
        /// <summary>
        /// Sends email notifications to the trainee, mentors, and admins when the state of a lesson changes.
        /// The email includes information about the old and new state, an optional rejection reason, and any submitted feedback.
        /// Notifications are sent only to users with open accounts and active notification settings.
        /// </summary>
        /// <param name="traineeLesson">The lesson whose state has changed, including trainee and lesson details.</param>
        /// <param name="oldState">The previous state of the lesson before the change.</param>
        /// <param name="newState">The new state of the lesson after the change.</param>
        /// <param name="feedback">
        /// Optional feedback submitted by the trainee if the TraineeLesson was set to rated.
        /// If present, the feedback details (difficulty, prior knowledge, effort, comment) are included in the email.
        /// </param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task NotifyAboutStateChangeAsync(TraineeLesson traineeLesson, TraineeLessonState oldState, TraineeLessonState newState, Feedback? feedback = null) {

            // +++++++++++++++
            // Get Trainee
            var trainee = await _databaseApplicationUserRepository.FindByIdWithNotificationSettingAsync(traineeLesson.TraineeId);

            var traineeSetting = trainee!.EmailNotificationSetting;
            string traineeName = trainee!.UserName!;
            string lessonTitle = traineeLesson.Lesson.Title;

            // +++++++++++++++
            // Rejected Note:
            string rejectedReason = traineeLesson.RejectionReason!;
            var rejectionNote = "";
            if (newState == TraineeLessonState.Rejected && !string.IsNullOrWhiteSpace(traineeLesson.RejectionReason)) {
                rejectionNote = $"<p><strong>Rejection Reason:</strong> {rejectedReason}</p>";
            }

            // +++++++++++++++
            // Feedback Note:
            string feedbackNote = "";
            if (feedback is not null) {
                feedbackNote = $@"
                    <hr/>
                    <p><strong>Feedback submitted by {traineeName}:</strong></p>
                    <ul>
                        <li><strong>Difficulty:</strong> {feedback.Difficulty.GetDisplayName()}</li>
                        <li><strong>Previous Knowledge:</strong> {feedback.PreviousKnowledge.GetDisplayName()}</li>
                        <li><strong>Hours of Effort:</strong> {feedback.HoursOfEffort} h</li>
                    </ul>";

                if (!string.IsNullOrWhiteSpace(feedback.Comment)) {
                    feedbackNote += $@"<p><strong>Comment:</strong><br/>{feedback.Comment}</p>";
                }
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // A. Notify Mentors and Admins
            var mentors = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Mentor");
            var admins = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Admin");

            // +++++++++++++++
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
                    <p>Hello {person.UserName},</p>
                    <p>The lesson <strong>“{lessonTitle}”</strong> of trainee <strong>{traineeName}</strong> has changed.</p>
                    <p><strong>Previous State:</strong> {oldState}<br/>
                    <strong>New State:</strong> {newState}</p>
                    {rejectionNote}
                    {feedbackNote}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(person.Email!, subject, messageHtml);
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // B. Notifiy Trainee
            if (!trainee.IsClosed && ShouldNotify(traineeSetting, newState)) {
                var subject = $"TraineeTracker: Lesson '{lessonTitle}' changed from {oldState} to {newState}";

                var messageHtml = $@"
                    <p>Hello {trainee.UserName},</p>
                    <p>The state of your lesson <strong>“{lessonTitle}”</strong> has changed.</p>
                    <p><strong>Previous State:</strong> {oldState}<br/>
                    <strong>New State:</strong> {newState}</p>
                    {rejectionNote}
                    {feedbackNote}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(trainee.Email!, subject, messageHtml);
            }
        }

        /// <summary>
        /// Sends email notifications to mentors, admins, and the trainee when feedback for a lesson is changed or deleted.
        /// The email includes a summary of the updated feedback or a deletion notice. 
        /// Notifications are only sent to users with open accounts and active notification settings.
        /// </summary>
        /// <param name="feedback">The feedback object containing lesson, author, and content details.</param>
        /// <param name="trueAuthor">The user who actually submitted or edited the feedback (may differ from the original author).</param>
        /// <param name="deleted">
        /// Indicates whether the feedback was deleted.
        /// If true, the notification states that the feedback was removed.
        /// If false, the updated feedback content is included in the message.
        /// </param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        // ------------------------------------------------------
        public async Task NotifyAboutFeedbackChangeAsync(Feedback feedback, ApplicationUser trueAuthor, bool deleted = false) {

            // +++++++++++++++
            // 1. Get Lesson
            var lesson = await _databaseLessonRepository
                .GetLessonByIdAsync(feedback.LessonId);

            // +++++++++++++++
            // 2. Get Trainee
            var trainee = await _databaseApplicationUserRepository
                .FindByIdWithNotificationSettingAsync(feedback.AuthorId);

            var traineeSetting = trainee!.EmailNotificationSetting;
            string traineeName = trainee!.UserName!;
            string lessonTitle = lesson!.Title;

            // +++++++++++++++
            // FeedbackNote:
            string feedbackNote = "";
            if (deleted) {
                feedbackNote = $@"
                    <hr/>
                    <p><strong>Updated Feedback:</strong> Feedback was deleted.</p>";
            } else {
                feedbackNote = $@"
                    <hr/>
                    <p><strong>Updated Feedback:</strong></p>
                    <ul>
                        <li><strong>Difficulty:</strong> {feedback.Difficulty.GetDisplayName()}</li>
                        <li><strong>Previous Knowledge:</strong> {feedback.PreviousKnowledge.GetDisplayName()}</li>
                        <li><strong>Hours of Effort:</strong> {feedback.HoursOfEffort} h</li>
                    </ul>";

                if (!string.IsNullOrWhiteSpace(feedback.Comment)) {
                    feedbackNote += $@"<p><strong>Comment:</strong><br/>{feedback.Comment}</p>";
                }
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // A. Notify Mentors and Admins
            var mentors = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Mentor");
            var admins = await _databaseApplicationUserRepository.GetOpenUsersInRoleWithEmailNotificationSettingAsync("Admin");

            var thirdPersons = mentors
                .Concat(admins)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var person in thirdPersons) {
                var setting = person.EmailNotificationSetting!;
                if (person.IsClosed || !setting.ReceiveFeedbackChangeNotifications)
                    continue;

                var subject = $"TraineeTracker: Feedback for lesson '{lessonTitle}' of '{trainee.UserName}'was changed";

                var messageHtml = $@"
                    <p>Hello {person.UserName},</p>
                    <p>The feedback for lesson <strong>“{lessonTitle}”</strong> from trainee <strong>{traineeName}</strong> was changed by <strong>{trueAuthor.UserName}</strong>.</p>
                    {feedbackNote}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(person.Email!, subject, messageHtml);
            }

            // ++++++++++++++++++++++++++++++++++++++++++
            // B. Notify Trainee (if someone else edited it)
            if (!trainee.IsClosed && feedback.AuthorId != trainee.Id && traineeSetting?.ReceiveFeedbackChangeNotifications == true) {
                var subject = $"TraineeTracker: Your feedback for lesson '{lessonTitle}' was changed";

                var messageHtml = $@"
                    <p>Hello {trainee.UserName},</p>
                    <p>Your feedback for the lesson <strong>“{lessonTitle}”</strong> was changed by <strong>“{trueAuthor.UserName}”</strong>.</p>
                    {feedbackNote}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                await _emailSender.SendEmailAsync(trainee.Email!, subject, messageHtml);
            }
        }

        // ------------------------------------------------------
        /// <summary>
        /// Sends email notifications about changes to a trainee's teaching plan during an import operation.
        /// Notifies the affected trainee as well as all mentors and admins with the appropriate settings.
        /// </summary>
        /// <param name="trainee">The trainee whose teaching plan has changed.</param>
        /// <param name="removedLessons">A list of lessons that have been removed from the trainee's plan.</param>
        /// <param name="addedLessons">A list of lessons that have been added to the trainee's plan.</param>
        /// <returns>
        /// A Task representing the asynchronous notification operation.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
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
            var changes = "";

            // ++++++++++++++++++++++++++++++++++++++++++
            // A. Notifiy Trainee
            var setting = await _databaseEmailNotificationSettingRepository.GetByUserIdAsync(trainee.Id);
            if (setting.ReceiveImportChangeNotifications && (added.Any() || removed.Any())) {
                var subject = "TraineeTracker: Your teaching plan has been updated";

                if (added.Any()) {
                    changes += "<p><strong>New lessons assigned:</strong><br/>" +
                            string.Join("<br/>", added.Select(n => $"– {n}")) + "</p>";
                }

                if (removed.Any()) {
                    changes += "<p><strong>Existing lessons unassigned:</strong><br/>" +
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
            // B. Notify Mentors and Admins
            if (added.Any() || removed.Any()) {
                var subject = $"TraineeTracker: Changes to {trainee.UserName}'s teachinglan";

                foreach (var person in thirdPersons) {
                    var s = person.EmailNotificationSetting!;
                    if (person.IsClosed || !s.ReceiveImportChangeNotifications)
                        continue;
                    var messageHtml = $@"
                    <p>Hello {person.UserName},</p>
                    <p>The following changes were made to <strong>{trainee.UserName}</strong>'s teaching plan during a teaching plan import:</p>
                    {changes}
                    <p>Best regards,<br/>Your TraineeTracker Team</p>";

                    await _emailSender.SendEmailAsync(person.Email!, subject, messageHtml);
                }
            }
        }

        // ------------------------------------------------------
        /// <summary>
        /// Creates a default set of email notification settings based on the user's role.
        /// Throws an ArgumentException if the role is unknown.
        /// </summary>
        /// <param name="role">The role of the user ("Trainee", "Mentor", or "Admin").</param>
        /// <returns>
        /// A new <see cref="EmailNotificationSetting"/> instance with role-specific defaults.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the role is not recognized.</exception>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public EmailNotificationSetting CreateDefaultEmailNotificationSetting(string role) {
            var setting = new EmailNotificationSetting();

            switch (role) {

                case "Admin":
                    setting.ReceiveSkippedNotifications = false;
                    setting.ReceiveOpenNotifications = false;
                    setting.ReceiveStartedNotifications = false;
                    setting.ReceiveFinishedNotifications = false;
                    setting.ReceiveRejectedNotifications = false;
                    setting.ReceiveAcceptedNotifications = false;
                    setting.ReceiveRatedNotifications = false;
                    setting.ReceiveImportChangeNotifications = false;
                    setting.ReceiveFeedbackChangeNotifications = false;
                    break;
                case "Mentor":
                    setting.ReceiveSkippedNotifications = false;
                    setting.ReceiveOpenNotifications = false;
                    setting.ReceiveStartedNotifications = false;
                    setting.ReceiveFinishedNotifications = true;
                    setting.ReceiveRejectedNotifications = false;
                    setting.ReceiveAcceptedNotifications = false;
                    setting.ReceiveRatedNotifications = true;
                    setting.ReceiveImportChangeNotifications = true;
                    setting.ReceiveFeedbackChangeNotifications = false;
                    break;
                case "Trainee":
                    setting.ReceiveSkippedNotifications = true;
                    setting.ReceiveOpenNotifications = false;
                    setting.ReceiveStartedNotifications = false;
                    setting.ReceiveFinishedNotifications = false;
                    setting.ReceiveRejectedNotifications = true;
                    setting.ReceiveAcceptedNotifications = true;
                    setting.ReceiveRatedNotifications = false;
                    setting.ReceiveImportChangeNotifications = true;
                    setting.ReceiveFeedbackChangeNotifications = false;
                    break;
                default:
                    throw new ArgumentException($"Unknown role '{role}' for default settings.");
            }

            return setting;
        }

        // ------------------------------------------------------
    }
}
