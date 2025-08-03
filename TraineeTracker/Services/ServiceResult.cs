namespace TraineeTracker.Models.Dtos {

    /// <summary>
    /// Represents the result of a service operation, indicating success or failure and containing error messages if any.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class ServiceResult {

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// Gets the collection of error messages associated with the operation.
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; private set; } = new List<string>();

        // ------------------------------------------------------
        /// <summary>
        /// Creates a successful <see cref="ServiceResult"/>.
        /// </summary>
        /// <returns>A <see cref="ServiceResult"/> indicating success.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public static ServiceResult Success() => new ServiceResult { Succeeded = true };

        // ------------------------------------------------------
        /// <summary>
        /// Creates a failed <see cref="ServiceResult"/> with the specified error messages.
        /// </summary>
        /// <param name="errorMessages">The error messages describing the failure.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating failure.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public static ServiceResult Failed(params string[] errorMessages) => new ServiceResult {
            Succeeded = false,
            ErrorMessages = errorMessages.ToList()
        };
    }

    /// <summary>
    /// Represents the result of a service operation, indicating success or failure, containing error messages if any, and optionally a value.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class ServiceResult<T> {

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// Gets the collection of error messages associated with the operation.
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; private set; } = new List<string>();

        /// <summary>
        /// Gets or sets the value returned by the operation, if any.
        /// </summary>
        public T? Value { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Creates a successful <see cref="ServiceResult{T}"/> without a value.
        /// </summary>
        /// <returns>A <see cref="ServiceResult{T}"/> indicating success.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public static ServiceResult<T> Success() => new ServiceResult<T> { Succeeded = true };

        // ------------------------------------------------------
        /// <summary>
        /// Creates a successful <see cref="ServiceResult{T}"/> with the specified value.
        /// </summary>
        /// <param name="value">The value returned by the operation.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> indicating success and containing the value.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public static ServiceResult<T> Success(T value) => new ServiceResult<T> { Succeeded = true, Value = value };

        // ------------------------------------------------------
        /// <summary>
        /// Creates a failed <see cref="ServiceResult{T}"/> with the specified error messages.
        /// </summary>
        /// <param name="errorMessages">The error messages describing the failure.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> indicating failure.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public static ServiceResult<T> Failed(params string[] errorMessages) => new ServiceResult<T> {
            Succeeded = false,
            ErrorMessages = errorMessages.ToList()
        };

        // ------------------------------------------------------
    }
}