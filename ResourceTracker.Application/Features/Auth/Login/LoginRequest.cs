using MediatR;
using ResourceTracker.Application.Models.Identity;

namespace ResourceTracker.Application.Features.Auth.Login
{
    public class LoginRequest : IRequest<AuthResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
