using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using Data.Models;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using AutoMapper;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly IProfessorService _professorService;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;
        //private readonly IMapper _professorService;

        public ProfessorsController(IProfessorService professorService, IMapper mapper, IAuthenticationService authenticationService)
        {
            _professorService = professorService;
            _mapper = mapper;
            _authenticationService = authenticationService;
        }

        // GET api/<ProfessorsController>/5
        /// <summary>
        /// Возвращает профессора по id
        /// </summary>
        /// <param name="id">id профессора</param>
        /// <returns></returns>
        /// <responce code="200"> Профессор успешно вывелся</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code = "404" > Профессор не найден></responce>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetProfessorViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfessorById(Guid id)
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var professorEntity = await _professorService.GetProfessorById(id).ConfigureAwait(false);
            if(professorEntity == null) return NotFound();

            return Ok(_mapper.Map<GetProfessorViewModel>(professorEntity));
        }

        /// <summary>
        /// Возвращает профиль профессора
        /// </summary>
        /// <returns></returns>
        /// <responce code="200"> Профессор успешно вывелся</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        [ProducesResponseType(typeof(GetProfessorViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [Domain.Services.Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfessorByJwt()
        {
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            return Ok(_mapper.Map<GetProfessorViewModel>(professor));
        }

        // POST api/<ProfessorsController>
        /// <summary>
        /// Добавление профессора
        /// </summary>
        /// <param name="createProfessorVM">Профессор</param>
        /// <returns></returns>
        /// <responce code="200"> Профессор успешно создан</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="422">Неверный объект</responce>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Domain.Services.Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProfessor([FromBody] CreateProfessorViewModel createProfessorVM)
        {
            if(!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            try
            {
                var createProfessor = _mapper.Map<Professor>(createProfessorVM);
                createProfessor.CreatedById = professor.Id;
                await _professorService.CreateProfessor(createProfessor).ConfigureAwait(false);
                return Ok();
            }
            catch(LoginExistException) 
            {
                return UnprocessableEntity(); 
            }
        }

        // PUT api/<ProfessorsController>/5
        /// <summary>
        /// Изменение профессора
        /// </summary>
        /// <param name="updateProfessorVM">Профессор</param>
        /// <returns></returns>
        /// <responce code="200"> Профессор успешно изменен</responce>
        /// <responce code="401">Профессор не авторизован</responce>
        /// <responce code="422">Неверный объект</responce>
        /// <responce code = "404" > Профессор не найден></responce>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Domain.Services.Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfessor(Guid id, [FromBody] UpdateProfessorViewModel updateProfessorVM)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var professor = (Professor)HttpContext.Items["User"];
            if (professor == null) return Unauthorized();

            var oldProfessor = await _professorService.GetProfessorById(id).ConfigureAwait(false);
            if (oldProfessor == null) return NotFound();

            if (professor.Id != id) return Forbid();

            try
            {
                var updateProfessor = _mapper.Map(updateProfessorVM, oldProfessor);

                await _professorService.UpdateProfessor(updateProfessor, updateProfessorVM.NewPassword, updateProfessorVM.NewPasswordRepeat).ConfigureAwait(false);
                return Ok();
            }
            catch (PasswordsNotEqualsException)
            {

                return UnprocessableEntity();
            }
            
        }
    }
}
