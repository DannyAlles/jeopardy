using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using Data.Models;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthorizationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Аутентифицирует преподававтеля
        /// </summary>
        /// <param name="authenticate">Логин и пароль</param>
        /// <returns>Токен</returns>
        /// <response code="200">Преподаватель успешно авторизирован</response>
        /// <response code="401">Преподаватель не может быть авторизирован</response>
        [ProducesResponseType(typeof(AuthenticateViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(GetAuthenticateViewModel authenticate)
        {
            try
            {
                var tokenViewModel = await _authenticationService.Authenticate(authenticate.Login, authenticate.Password).ConfigureAwait(false);

                if (tokenViewModel == null)
                    return Unauthorized();

                return Ok(new AuthenticateViewModel() { Token = tokenViewModel.Token });
            }
            catch (ProfessorNotFoundException)
            {
                return Unauthorized();
            }
            catch (PasswordsNotEqualsException)
            {
                return Unauthorized();
            }
            
        }
    }
}