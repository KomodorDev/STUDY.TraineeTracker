namespace TraineeTracker.Models.Dtos {
    //
    // Summary:
    //     Repräsentiert das Ergebnis einer Service Operation
    public class ServiceResult {
        public bool Succeeded { get; private set; }

        public IEnumerable<string> ErrorMessages { get; private set; } = new List<string>();

        public static ServiceResult Success() => new ServiceResult { Succeeded = true };
        public static ServiceResult Failed(params string[] errorMessages) => new ServiceResult {
            Succeeded = false,
            ErrorMessages = errorMessages.ToList()
        };
    }
}