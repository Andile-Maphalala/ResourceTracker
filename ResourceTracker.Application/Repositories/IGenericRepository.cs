using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Repositories
{
    public interface IGenericRepository
    {
        IQueryable<T> Set<T>() where T : class;
        Task InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
        Task UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
        Task BulkUpdateAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class;
        Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

    }
}
