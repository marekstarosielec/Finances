using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GasController : ControllerBase
    {
        private readonly IGasService _gasService;

        public GasController(IGasService gasService)
        {
            _gasService = gasService;
        }

        [HttpGet]
        public IEnumerable<Gas> Get() => _gasService.GetGas();

        [HttpPost("gas")]
        public IActionResult Add([FromBody] Gas gas)
        {
            _gasService.SaveGas(gas);
            return Ok();
        }

        [HttpPut("gas")]
        public IActionResult Save([FromBody] Gas gas)
        {
            _gasService.SaveGas(gas);
            return Ok();
        }

        [HttpDelete("gas/{id}")]
        public IActionResult Delete(string id)
        {
            _gasService.DeleteGas(id);
            return Ok();
        }
    }
}
