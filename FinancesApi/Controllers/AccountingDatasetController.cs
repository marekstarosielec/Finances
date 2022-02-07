using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountingDatasetController : ControllerBase
    {
        private readonly IAccountingDatasetService _accountingDatasetService;

        public AccountingDatasetController(IAccountingDatasetService accountingDatasetService)
        {
            _accountingDatasetService = accountingDatasetService;
        }

        [HttpGet]
        public AccountingDatasetInfo GetDatasetInfo() => _accountingDatasetService.GetInfo();

        [HttpPost("Open")]
        public IActionResult OpenDataset([FromBody] DatasetOpenInstruction instruction)
        {
            return Ok(_accountingDatasetService.Open(instruction.Password));
        }

        [HttpPost("Close")]
        public IActionResult CloseDataset([FromBody] DatasetCloseInstruction instruction)
        {
            return Ok(_accountingDatasetService.Close(instruction.Password, instruction.MakeBackups));
        }

        [HttpPost("Execute")]
        public IActionResult Execute()
        {
            _accountingDatasetService.Execute();
            return Ok();
        }
    }
}
