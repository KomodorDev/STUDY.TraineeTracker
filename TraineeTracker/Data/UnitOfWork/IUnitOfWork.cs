namespace TraineeTracker.Data.UnitOfWork {

    /// <summary>
    /// Defines the contract for a unit of work, providing methods to manage database transactions.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public interface IUnitOfWork {

        // ------------------------------------------------------
        /// <summary>
        /// Begins a new database transaction asynchronously.
        /// </summary>
        Task BeginTransactionAsync();

        // ------------------------------------------------------
        /// <summary>
        /// Commits the current transaction asynchronously.
        /// </summary>
        Task CommitAsync();

        // ------------------------------------------------------
        /// <summary>
        /// Rolls back the current transaction asynchronously.
        /// </summary>
        Task RollbackAsync();

        // ------------------------------------------------------
    }
}