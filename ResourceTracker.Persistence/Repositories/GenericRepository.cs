using Microsoft.EntityFrameworkCore;
using ResourceTracker.Persistence.Data;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Application.Common.Exceptions;

using System.Linq.Expressions;
using EFCore.BulkExtensions;

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

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) where T : class
        {
            var obj = await Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);

            if (obj == null)
            {
                throw new BadRequestException("Record not found");
            }
            _dbContext.Remove(obj);
        }

        public async Task InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : class
        {
            await _dbContext.AddAsync(entity);
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
    }
}
