using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutoringListController : ControllerBase
    {
        private readonly ITutoringListService _tutoringListService;

        public TutoringListController(ITutoringListService tutoringListService)
        {
            _tutoringListService = tutoringListService;
        }

        [HttpGet]
        public IEnumerable<TutoringList> Get() => _tutoringListService.GetTutoringList();

        [HttpPost("tutoringList")]
        public IActionResult Add([FromBody] TutoringList tutoringList)
        {
            _tutoringListService.SaveTutoringList(tutoringList);
            return Ok();
        }

        [HttpPut("tutoringList")]
        public IActionResult Save([FromBody] TutoringList tutoringList)
        {
            _tutoringListService.SaveTutoringList(tutoringList);
            return Ok();
        }

        [HttpDelete("tutoringList/{id}")]
        public IActionResult Delete(string id)
        {
            _tutoringListService.DeleteTutoringList(id);
            return Ok();
        }
    }
}
