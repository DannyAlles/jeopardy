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
    public class ThemesController : ControllerBase
    {
        private readonly IThemeService _themeService;
        private readonly IMapper _mapper;

        public ThemesController(IThemeService themeService, IMapper mapper)
        {
            _themeService = themeService;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает тему
        /// </summary>
        /// <param name="createThemeVM">Категория</param>
        /// <returns></returns>
        /// <responce code="200">Категория успешно создана</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="422">Неверный объект</responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Domain.Services.Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTheme([FromBody] CreateThemeViewModel createThemeVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var createTheme = _mapper.Map<Theme>(createThemeVM);

            var themeEntity = await _themeService.GetThemeByName(createTheme.Title).ConfigureAwait(false);
            if (themeEntity != null) return UnprocessableEntity();

            await _themeService.CreateTheme(createTheme).ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Выдает список тем включающий в себя подстроку
        /// </summary>
        /// <param name="title">Подстрока</param>
        /// <returns></returns>
        /// <responce code="200">Список составился успешно</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        [ProducesResponseType(typeof(GetThemeListViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet]
        public async Task<IActionResult> GetThemeList([FromQuery] string? title)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var themes = await _themeService.GetThemeList(title).ConfigureAwait(false);

            return Ok(_mapper.Map<IEnumerable<GetThemeListViewModel>>(themes));
        }
    }
}
