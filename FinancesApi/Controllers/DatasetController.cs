using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DatasetController : ControllerBase
    {
        private readonly IDatasetService datasetService;

        public DatasetController(IDatasetService datasetService)
        {
            this.datasetService = datasetService;
        }

        [HttpGet]
        public DatasetInfo GetDatasetInfo() => datasetService.GetInfo();

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
