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
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IMapper _mapper;

        public QuestionsController(IQuestionService questionService, IMapper mapper)
        {
            _questionService = questionService;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает вопрос
        /// </summary>
        /// <param name="createQuestionVM">Вопрос</param>
        /// <returns></returns>
        /// <responce code="200">Вопрос успешно создан</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="422">Неверный объект</responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Domain.Services.Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionViewModel createQuestionVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var createQuestion = _mapper.Map<Question>(createQuestionVM);

            await _questionService.CreateQuestion(createQuestion).ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Возвращает вопрос
        /// </summary>
        /// <param name="id">id вопроса</param>
        /// <returns></returns>
        /// <responce code="200">Вопрос успешно передан</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code = "404" > Вопрос не найден></responce>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetQuestionViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var questionEntity = await _questionService.GetQuestion(id).ConfigureAwait(false);
            if (questionEntity == null) return NotFound();

            return Ok(_mapper.Map<GetQuestionViewModel>(questionEntity));

        }

        /// <summary>
        /// Возвращает список вопросов по названию
        /// </summary>
        /// <param name="text">Текст вопроса</param>
        /// <returns></returns>
        /// <responce code="200">Список вопросов успешно передан</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        [ProducesResponseType(typeof(GetQuestionListViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("GetByText")]
        public async Task<IActionResult> GetQuestionList([FromQuery] string? text)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var quetion = await _questionService.GetQuestionList(text).ConfigureAwait(false);

            return Ok(_mapper.Map<IEnumerable<GetQuestionListViewModel>>(quetion));
        }

        /// <summary>
        /// Возвращает список вопросов по названию темы
        /// </summary>
        /// <param name="title">Название темы</param>
        /// <returns></returns>
        /// <responce code="200">Список вопросов успешно передан</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        [ProducesResponseType(typeof(GetQuestionListViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("GetByTheme")]
        public async Task<IActionResult> GetQuestionOfThemeList([FromQuery] string? title)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var quetion = await _questionService.GetQuestionOfThemeList(title).ConfigureAwait(false);

            return Ok(_mapper.Map<IEnumerable<GetQuestionListViewModel>>(quetion));
        }

        /// <summary>
        /// Изменяет вопрос
        /// </summary>
        /// <param name = "updateQuestionVM" > Вопрос </param >
        /// <returns></returns>
        /// <responce code="200">Вопрос успешно изменился</responce>
        /// <responce code = "401" > Профессор не авторизован</responce>
        /// <responce code = "404" > Вопрос не найден></responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [Domain.Services.Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuestionViewModel updateQuestionVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var oldQuestion = await _questionService.GetQuestion(id).ConfigureAwait(false);
            if (oldQuestion == null) return NotFound();

            var updateQuestion = _mapper.Map(updateQuestionVM, oldQuestion);
            await _questionService.UpdateQuestion(updateQuestion).ConfigureAwait(false);
            return Ok();
        }
    }
}
