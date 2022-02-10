using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ElectricityController : ControllerBase
    {
        private readonly IElectricityService _electricityService;

        public ElectricityController(IElectricityService electricityService)
        {
            _electricityService = electricityService;
        }

        [HttpGet]
        public IEnumerable<Electricity> Get() => _electricityService.GetElectricity();

        [HttpPost("electricity")]
        public IActionResult Add([FromBody] Electricity electricity)
        {
            _electricityService.SaveElectricity(electricity);
            return Ok();
        }

        [HttpPut("electricity")]
        public IActionResult Save([FromBody] Electricity electricity)
        {
            _electricityService.SaveElectricity(electricity);
            return Ok();
        }

        [HttpDelete("electricity/{id}")]
        public IActionResult Delete(string id)
        {
            _electricityService.DeleteElectricity(id);
            return Ok();
        }
    }
}
