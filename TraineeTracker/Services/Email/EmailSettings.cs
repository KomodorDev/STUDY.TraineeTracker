
/// <summary>
/// Represents the configuration settings used for sending emails, including sender address and password.
/// These values are loaded via dependency injection from <c>appsettings.json</c>
/// using configuration binding.
/// </summary>
/// <remarks>
/// Code Ownership: Simon Hinterreiter (hintsimo)
/// </remarks>
public class EmailSettings {

    /// <summary>
    /// The email address from which notifications are sent.
    /// </summary>
    public required string From { get; set; }

    /// <summary>
    /// The password or application-specific token for the sender's email account.
    /// </summary>
    public required string Password { get; set; }
}
