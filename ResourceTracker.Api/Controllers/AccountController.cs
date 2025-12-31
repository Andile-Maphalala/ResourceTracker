using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Auth.Login;
using ResourceTracker.Application.Features.Auth.Register;
using ResourceTracker.Application.Models.Identity;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public AccountController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FastLogin")]
        public async Task<ActionResult<string>> FastLogin()
        {

            var email = _configuration["DevCredentials:Email"];
            var password = _configuration["DevCredentials:Password"];

            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };
            var response = await _mediator.Send(request);


            var bearer = response.Token;

            return Ok(bearer);
        }
    }
}
