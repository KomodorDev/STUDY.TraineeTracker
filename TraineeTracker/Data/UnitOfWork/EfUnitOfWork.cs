using Microsoft.EntityFrameworkCore.Storage;

namespace TraineeTracker.Data.UnitOfWork {

    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface using Entity Framework Core.
    /// Manages database transactions and ensures atomic operations within the <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class EfUnitOfWork : IUnitOfWork {

        // ------------------------------------------------------
        /// <summary>
        /// The application's database context used for data operations.
        /// </summary>
        private readonly ApplicationDbContext _context;

        // ------------------------------------------------------
        /// <summary>
        /// The current database transaction, if any.
        /// </summary>
        private IDbContextTransaction? _transaction;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWork"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> to be used for data operations.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public EfUnitOfWork(ApplicationDbContext context) {
            _context = context;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Begins a new database transaction asynchronously, unless using the in-memory provider.
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task BeginTransactionAsync() {
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory") {
                return;
            }
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Commits the current transaction asynchronously, if one exists.
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task CommitAsync() {
            if (_transaction != null) {
                await _transaction.CommitAsync();
            }
        }

        // ------------------------------------------------------
        /// <summary>
        /// Rolls back the current transaction asynchronously and resets the transaction state, if one exists.
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task RollbackAsync() {
            if (_transaction != null) {
                await _transaction.RollbackAsync();
                _transaction = null;
            }
        }

        // ------------------------------------------------------
    }
}