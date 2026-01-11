using ResourceTracker.Application.Interfaces;
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

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
