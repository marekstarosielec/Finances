using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettlementController : ControllerBase
    {
        private readonly ISettlementService _settlementService;

        public SettlementController(ISettlementService settlementService)
        {
            _settlementService = settlementService;
        }

        [HttpGet]
        public IEnumerable<Settlement> Get() => _settlementService.Get();

        [HttpPost("settlement")]
        public IActionResult Add([FromBody] Settlement model)
        {
            _settlementService.Save(model);
            return Ok();
        }

        [HttpPut("settlement")]
        public IActionResult Save([FromBody] Settlement model)
        {
            _settlementService.Save(model);
            return Ok();
        }

        [HttpDelete("settlement/{id}")]
        public IActionResult Delete(string id)
        {
            _settlementService.Delete(id);
            return Ok();
        }
    }
}
