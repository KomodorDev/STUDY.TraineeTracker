namespace TraineeTracker.Models.Dtos {
    public class ServiceResult {
        public bool Succeeded { get; private set; }
        public IEnumerable<string> ErrorMessages { get; private set; } = new List<string>();

        public static ServiceResult Success() => new ServiceResult { Succeeded = true };
        public static ServiceResult Failed(params string[] errorMessages) => new ServiceResult {
            Succeeded = false,
            ErrorMessages = errorMessages.ToList()
        };
    }

    public class ServiceResult<T> {
        public bool Succeeded { get; private set; }
        public IEnumerable<string> ErrorMessages { get; private set; } = new List<string>();
        public T? Value { get; set; }

        public static ServiceResult<T> Success() => new ServiceResult<T> { Succeeded = true };
        public static ServiceResult<T> Success(T value) => new ServiceResult<T> { Succeeded = true, Value = value };
        public static ServiceResult<T> Failed(params string[] errorMessages) => new ServiceResult<T> {
            Succeeded = false,
            ErrorMessages = errorMessages.ToList()
        };
    }
}