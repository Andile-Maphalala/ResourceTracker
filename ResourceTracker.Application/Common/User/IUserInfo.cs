
namespace ResourceTracker.Application.Common.User
{
    public interface IUserInfo
    {
        int GetUserId();

        bool IsAdmin();
        bool IsLoginAsActive();
    }
}
