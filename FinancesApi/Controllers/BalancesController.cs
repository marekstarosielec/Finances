using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BalancesController : ControllerBase
    {
        private readonly ILogger<BalancesController> _logger;
        private readonly IBalanceService _balancesService;

        public BalancesController(ILogger<BalancesController> logger, IBalanceService balancesService)
        {
            _logger = logger;
            _balancesService = balancesService;
        }

        [HttpGet]
        public IEnumerable<Balance> Get() => _balancesService.GetBalances();

        [HttpGet("{id}")]
        public Balance GetSingle(string id) => _balancesService.GetBalances(id).FirstOrDefault();

        [HttpPost("balance")]
        public IActionResult Add([FromBody] Balance balance)
        {
            _balancesService.SaveBalance(balance);
            return Ok();
        }

        [HttpPut("balance")]
        public IActionResult Save([FromBody] Balance balance)
        {
            _balancesService.SaveBalance(balance);
            return Ok();
        }

        [HttpDelete("balance/{id}")]
        public IActionResult Delete(string id)
        {
            _balancesService.DeleteBalance(id);
            return Ok();
        }
    }
}
