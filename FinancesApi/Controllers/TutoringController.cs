using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutoringController : ControllerBase
    {
        private readonly ITutoringService _tutoringService;

        public TutoringController(ITutoringService tutoringService)
        {
            _tutoringService = tutoringService;
        }

        [HttpGet]
        public IEnumerable<Tutoring> Get() => _tutoringService.GetTutoring();

        [HttpPost("tutoring")]
        public IActionResult Add([FromBody] Tutoring tutoring)
        {
            _tutoringService.SaveTutoring(tutoring);
            return Ok();
        }

        [HttpPut("tutoring")]
        public IActionResult Save([FromBody] Tutoring tutoring)
        {
            _tutoringService.SaveTutoring(tutoring);
            return Ok();
        }

        [HttpDelete("tutoring/{id}")]
        public IActionResult Delete(string id)
        {
            _tutoringService.DeleteTutoring(id);
            return Ok();
        }
    }
}
