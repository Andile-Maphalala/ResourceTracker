using EntitySecurity.Contract.Security;

namespace ResourceTracker.Api.Middleware
{
    public class UserIdentifierMiddleware
    {
        private readonly RequestDelegate _next;

        public UserIdentifierMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, IInfoSetter infoSetter)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var claims = context.User.Claims;

                infoSetter.SetUser(claims);
            }

            return _next(context);
        }
    }
}
