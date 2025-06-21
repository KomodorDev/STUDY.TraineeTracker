namespace TraineeTracker.Models.Dtos {
    //
    // Summary:
    //     Repräsentiert das Ergebnis einer Service Operation
    public class ServiceResult {
        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; } = "";

        public static ServiceResult Success() => new ServiceResult { Succeeded = true };
        public static ServiceResult Failed(string errorMessage) => new ServiceResult {
            Succeeded = false,
            ErrorMessage = errorMessage
        };
    }
}