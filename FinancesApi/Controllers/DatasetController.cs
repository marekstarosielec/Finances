using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DatasetController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public DatasetController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("IsOpen")]
        public bool IsDatasetOpen()
        {
            var path = configuration.GetValue<string>("DatasetPath");
            return System.IO.File.Exists(Path.Combine(path, "transactions.json"));
        }

        [HttpPost("Open")]
        public IActionResult OpenDataset()
        {
            return Ok();
        }

        [HttpPost("Close")]
        public IActionResult CloseDataset()
        {
            return Ok();
        }

    }
}
