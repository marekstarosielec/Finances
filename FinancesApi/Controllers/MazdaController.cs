using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MazdaController : ControllerBase
    {
        private readonly ILogger<BalancesController> _logger;
        private readonly IMazdaService _mazdaService;

        public MazdaController(ILogger<BalancesController> logger, IMazdaService mazdaService)
        {
            _logger = logger;
            _mazdaService = mazdaService;
        }

        [HttpGet]
        public IEnumerable<Mazda> Get() => _mazdaService.GetMazda();

        [HttpPost("mazda")]
        public IActionResult Add([FromBody] Mazda mazda)
        {
            _mazdaService.SaveMazda(mazda);
            return Ok();
        }

        [HttpPut("mazda")]
        public IActionResult Save([FromBody] Mazda mazda)
        {
            _mazdaService.SaveMazda(mazda);
            return Ok();
        }

        [HttpDelete("mazda/{id}")]
        public IActionResult Delete(string id)
        {
            _mazdaService.DeleteMazda(id);
            return Ok();
        }
    }
}
