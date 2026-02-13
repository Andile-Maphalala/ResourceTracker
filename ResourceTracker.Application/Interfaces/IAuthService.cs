using ResourceTracker.Application.Models.Identity;

namespace ResourceTracker.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegistrationResponse> Register(RegistrationRequest request);
    }
}
