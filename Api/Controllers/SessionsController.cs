using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using Data.Models;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using AutoMapper;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;

        public SessionsController(ISessionService sessionService, IMapper mapper)
        {
            _sessionService = sessionService;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает сессию
        /// </summary>
        /// <param name="createSessionVM">Сессия</param>
        /// <returns></returns>
        /// <responce code="200">Сессия успешно создана</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="422">Неверный объект</responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Domain.Services.Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionViewModel createSessionVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var createSession = _mapper.Map<Session>(createSessionVM);

            //var sessioEntity = await _sessionService.TeamEntity(createTheme.Title, createTheme.SessionId).ConfigureAwait(false);
            //if (sessioEntity != null) return UnprocessableEntity();

            await _sessionService.CreateSession(createSession, professor).ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Возвращает сессию
        /// </summary>
        /// <param name="id">id сессии</param>
        /// <returns></returns>
        /// <responce code="200">успешно</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code = "404" > Сессия не найдена></responce>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetSessionViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(Guid id)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var session = await _sessionService.GetSession(id).ConfigureAwait(false);
            if(session == null) return NotFound();
            return Ok(_mapper.Map<GetSessionViewModel>(session));
        }
    }
}
