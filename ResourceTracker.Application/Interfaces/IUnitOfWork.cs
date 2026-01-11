
namespace ResourceTracker.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task Save(CancellationToken cancellationToken);
    }
}
