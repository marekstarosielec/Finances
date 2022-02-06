using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SkodaController : ControllerBase
    {
        private readonly ILogger<BalancesController> _logger;
        private readonly ISkodaService _skodaService;

        public SkodaController(ILogger<BalancesController> logger, ISkodaService skodaService)
        {
            _logger = logger;
            _skodaService = skodaService;
        }

        [HttpGet]
        public IEnumerable<Skoda> Get() => _skodaService.GetSkoda();

        [HttpPost("skoda")]
        public IActionResult Add([FromBody] Skoda skoda)
        {
            _skodaService.SaveSkoda(skoda);
            return Ok();
        }

        [HttpPut("skoda")]
        public IActionResult Save([FromBody] Skoda skoda)
        {
            _skodaService.SaveSkoda(skoda);
            return Ok();
        }

        [HttpDelete("skoda/{id}")]
        public IActionResult Delete(string id)
        {
            _skodaService.DeleteSkoda(id);
            return Ok();
        }
    }
}
