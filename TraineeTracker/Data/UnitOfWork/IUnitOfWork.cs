namespace TraineeTracker.Data.UnitOfWork
{
    public interface IUnitOfWork {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}