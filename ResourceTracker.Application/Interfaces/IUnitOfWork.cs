
namespace ResourceTracker.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task Save(CancellationToken cancellationToken);

        /// <summary>
        /// Runs <paramref name="action"/> inside a database transaction, committing on success and
        /// rolling back on any exception. Needed for operations that must not be left half-applied,
        /// such as a purge.
        /// </summary>
        Task<T> ExecuteInTransaction<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken);
    }
}
