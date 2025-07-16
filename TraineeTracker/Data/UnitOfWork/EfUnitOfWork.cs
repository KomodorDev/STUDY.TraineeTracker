using Microsoft.EntityFrameworkCore.Storage;

namespace TraineeTracker.Data.UnitOfWork
{
    public class EfUnitOfWork : IUnitOfWork {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public EfUnitOfWork(ApplicationDbContext context) {
            _context = context;
        }

        public async Task BeginTransactionAsync() {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync() {
            if (_transaction != null) {
                await _transaction.CommitAsync();
            }
        }

        public async Task RollbackAsync() {
            if (_transaction != null) {
                await _transaction.RollbackAsync();
            }
        }
    }
}