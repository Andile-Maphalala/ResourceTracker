using ResourceTracker.Application.Repositories;
using ResourceTracker.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ResourceTrackerDbContext _context;
        private IResourceTrackerRepository _repository;

        public UnitOfWork(ResourceTrackerDbContext context)
        {
            _context = context;
        }

        public IResourceTrackerRepository Repository => _repository ??= new ResourceTrackerRepository(_context);

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
