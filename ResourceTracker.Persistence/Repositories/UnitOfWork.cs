using ResourceTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Persistence.Data;

namespace ResourceTracker.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ResourceTrackerDbContext _context;
        public UnitOfWork(ResourceTrackerDbContext context)
        {
            _context = context;
        }

        public async Task Save(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> ExecuteInTransaction<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await action(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
