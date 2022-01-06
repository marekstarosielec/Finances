using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DocumentDatasetController : ControllerBase
    {
        private readonly IDocumentDatasetService _documentDatasetService;

        public DocumentDatasetController(IDocumentDatasetService documentDatasetService)
        {
            _documentDatasetService = documentDatasetService;
        }

        [HttpGet]
        public DocumentDatasetInfo GetDatasetInfo() => _documentDatasetService.GetInfo();

        [HttpPost("Open")]
        public IActionResult OpenDataset([FromBody] DatasetOpenInstruction instruction)
        {
            return Ok(_documentDatasetService.Open(instruction.Password));
        }

        [HttpPost("Close")]
        public IActionResult CloseDataset([FromBody] DatasetCloseInstruction instruction)
        {
            return Ok(_documentDatasetService.Close(instruction.Password, instruction.MakeBackups));
        }

    }
}
