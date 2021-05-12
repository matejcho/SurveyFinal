using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using midTerm.Models.Models.SurveyUser;
using midTerm.Services.Abstractions;

namespace midTerm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class SurveyUserController 
        : ControllerBase
    {
        private readonly ISurveyUserService _service;

        public SurveyUserController(ISurveyUserService service)
        {
            _service = service;
        }
        [HttpGet("/SurveyUser", Name = nameof(Get))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SurveyUserBaseModel>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _service.Get();
            return Ok(result);
        }

        [HttpGet("/SurveyUser/{id:int}", Name = nameof(Get))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SurveyUserExtended))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] SurveyUserCreate model)
        {
            if (ModelState.IsValid)
            {
                var user = await _service.Insert(model);
                return user != null
                    ? (IActionResult)CreatedAtRoute(nameof(GetById), user, user.Id)
                    : Conflict();
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SurveyUserBaseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] SurveyUserUpdate model)
        {
            if (ModelState.IsValid)
            {
                model.Id = id;
                var result = await _service.Update(model);

                return result != null
                    ? (IActionResult)Ok(result)
                    : NoContent();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _service.Delete(id));
            }
            return BadRequest();
        }
    }
}
