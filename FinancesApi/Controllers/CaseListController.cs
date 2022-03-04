using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CaseListController : ControllerBase
    {
        private readonly ILogger<BalancesController> _logger;
        private readonly ICaseListService _caseListService;

        public CaseListController(ILogger<BalancesController> logger, ICaseListService caseListService)
        {
            _logger = logger;
            _caseListService = caseListService;
        }

        [HttpGet]
        public IEnumerable<CaseList> Get() => _caseListService.GetCaseList();

        [HttpPost("case")]
        public IActionResult Add([FromBody] CaseList caseList)
        {
            _caseListService.SaveCaseList(caseList);
            return Ok();
        }

        [HttpPut("case")]
        public IActionResult Save([FromBody] CaseList caseList)
        {
            _caseListService.SaveCaseList(caseList);
            return Ok();
        }

        [HttpDelete("case/{id}")]
        public IActionResult Delete(string id)
        {
            _caseListService.DeleteCaseList(id);
            return Ok();
        }
    }
}
