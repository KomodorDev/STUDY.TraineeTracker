namespace TraineeTracker.Data
{
    public interface IUnitOfWork {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}