

namespace ResourceTracker.Application.Interfaces
{
    public interface IGenericRepository
    {
        IQueryable<T> Set<T>() where T : class;
        Task InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
        Task BulkInsertAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class;
        Task BulkInsertAndUpdateIdsAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class;
        Task UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
        Task BulkUpdateAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class;
        Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    }
}
