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
        private readonly IDatasetService _datasetService;

        public DatasetController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        [HttpGet]
        public DatasetInfo GetDatasetInfo() => _datasetService.GetInfo();

        [HttpPost("Open")]
        public IActionResult OpenDataset()
        {
            return Ok(_datasetService.Open());
        }

        [HttpPost("Close")]
        public IActionResult CloseDataset()
        {
            return Ok(_datasetService.Close());
        }

    }
}
