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
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamsController(ITeamService teamService, IMapper mapper)
        {
            _teamService = teamService;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает Команду
        /// </summary>
        /// <param name="createTeamVM">Команда</param>
        /// <returns></returns>
        /// <responce code="200">Команда успешно создана</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="422">Неверный объект</responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Domain.Services.Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamViewModel createTeamVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var createTheme = _mapper.Map<Team>(createTeamVM);

            var teamEntity = await _teamService.TeamEntity(createTheme.Title, createTheme.SessionId).ConfigureAwait(false);
            if (teamEntity != null) return UnprocessableEntity();

            await _teamService.CreateTeam(createTheme).ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Изменяет Команду
        /// </summary>
        /// <param name="updateTeamVM">Команда</param>
        /// <returns></returns>
        /// <responce code="200">Команда изменена</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="404"> Команда не найдена</responce>
        /// <responce code="422">Неверный объект</responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Domain.Services.Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamViewModel updateTeamVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var oldTeam = await _teamService.GetTeamById(id).ConfigureAwait(false);
            if (oldTeam == null) return NotFound();

            var updateTeam = _mapper.Map(updateTeamVM, oldTeam);

            await _teamService.UpdateTeam(updateTeam).ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Возвращает комаду
        /// </summary>
        /// <param name="id">id команды</param>
        /// <returns></returns>
        /// <responce code="200">успешно</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code = "404" > Команда не найдена></responce>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetTeamViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(Guid id)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var team = await _teamService.GetTeamById(id).ConfigureAwait(false);
            if (team == null) return NotFound();
            return Ok(_mapper.Map<GetTeamViewModel>(team));
        }
    }
}
