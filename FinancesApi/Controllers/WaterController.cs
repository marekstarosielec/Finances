using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WaterController : ControllerBase
    {
        private readonly IWaterService _service;

        public WaterController(IWaterService service)
        {
            _service = service;
        }

        [HttpGet]
        public IEnumerable<Water> Get() => _service.Get();

        [HttpPost("water")]
        public IActionResult Add([FromBody] Water model)
        {
            _service.Save(model);
            return Ok();
        }

        [HttpPut("water")]
        public IActionResult Save([FromBody] Water model)
        {
            _service.Save(model);
            return Ok();
        }

        [HttpDelete("water/{id}")]
        public IActionResult Delete(string id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
