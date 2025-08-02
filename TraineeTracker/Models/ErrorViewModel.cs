/// <summary>
/// ViewModel for representing error information in the application.
/// </summary>
public class ErrorViewModel {
    /// <summary>
    /// Gets or sets the unique identifier for the current request.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="RequestId"/> should be shown.
    /// Returns <c>true</c> if <see cref="RequestId"/> is not null or empty; otherwise, <c>false</c>.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
