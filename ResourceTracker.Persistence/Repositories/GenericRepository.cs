using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Persistence.Data;

namespace ResourceTracker.Persistence.Repositories
{
    public class GenericRepository : IGenericRepository
    {
        private readonly ResourceTrackerDbContext _dbContext;

        public GenericRepository(ResourceTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Set<T>() where T : class
        {

            return _dbContext.Set<T>();
        }

        public async Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbContext.Remove(entity);
        }

        public async Task InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : class
        {

            await _dbContext.AddAsync(entity);
        }

        public async Task BulkInsertAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class
        {
            if (entities.Any())
            {
                await _dbContext.BulkInsertAsync(entities);
            }
        }

        public async Task UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : class
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task BulkUpdateAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class
        {
            if (entities.Any())
            {
                await _dbContext.BulkUpdateAsync(entities);
            }
        }

        public async Task BulkInsertAndUpdateIdsAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class
        {
            if (entities.Any())
            {
                await _dbContext.BulkInsertAsync(entities, options =>
                {
                    options.SetOutputIdentity = true;
                });
            }
        }
    }
}
