using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using Data.Models;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using AutoMapper;
using System.Transactions;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly IQuestionOfPackageService _questionOfPackageService;
        private readonly IPackageService _packageService;
        private readonly IMapper _mapper;

        public PackagesController(IQuestionOfPackageService questionOfPackageService,
                                  IPackageService packageService, IMapper mapper)
        {
            _questionOfPackageService = questionOfPackageService;
            _packageService = packageService;
            _mapper = mapper;
        }

        /// <summary>
        /// Добавляет пак
        /// </summary>
        /// <param name="createPackageVM">Пакет</param>
        /// <returns></returns>
        /// <responce code="200">Пак добавлен успешно</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="422">Неверный объект</responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        ////[Domain.Services.Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuestionOfPackage([FromBody] CreatePackageViewModel createPackageVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var package = _mapper.Map<Package>(createPackageVM);
            await _packageService.CreatePackage(package, professor);
            return Ok();
        }

        /// <summary>
        /// Возвращает пакет по id
        /// </summary>
        /// <param name="id">id пакета</param>
        /// <returns></returns>
        /// <responce code="200">Пакет успешно найден</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code = "404" > Пакет не найден></responce>
        [ProducesResponseType(typeof(GetPackageListViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [Domain.Services.Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageById(Guid id)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var package = await _packageService.GetPackage(id).ConfigureAwait(false);
            if(package == null) return NotFound();

            var pack = _mapper.Map<GetPackageListViewModel>(package);
            pack.x = await _packageService.GetPackageX(id);
            pack.y = await _packageService.GetPackageY(id);

            return Ok(pack);

        }

        /// <summary>
        /// Возвращает список пакетов по части названия пакета
        /// </summary>
        /// <param name="title">часть названия пакета</param>
        /// <returns></returns>
        /// <responce code="200">Список успешно составлен</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        [ProducesResponseType(typeof(GetPackageListViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetPackageList([FromQuery] string? title)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var package = await _packageService.GetPackageList(title).ConfigureAwait(false);

            return Ok(_mapper.Map<IEnumerable<GetPackageListViewModel>>(package));

        }

        /// <summary>
        /// Выдает список вопросов в паке
        /// </summary>
        /// <param name="id">id пака</param>
        /// <returns></returns>
        /// <responce code="200">Список составился успешно</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code = "404" > Пакет не найден></responce>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<GetQuestionOfPackageListViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("{id}/Questions")]
        public async Task<IActionResult> GetQuestionsOfPackage(Guid id)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var questionOfPackage = await _questionOfPackageService.GetQuestionOfPackageList(id).ConfigureAwait(false);
            if(questionOfPackage == null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<GetQuestionOfPackageListViewModel>>(questionOfPackage));

        }
    }
}
