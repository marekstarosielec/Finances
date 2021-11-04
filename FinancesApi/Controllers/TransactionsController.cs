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
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly ITransactionsService _transactionsService;

        public TransactionsController(ILogger<TransactionsController> logger, ITransactionsService transactionsService)
        {
            _logger = logger;
            _transactionsService = transactionsService;
        }

        [HttpGet]
        public IEnumerable<Transaction> Get() => _transactionsService.GetTransactions();

        [HttpGet("{id}")]
        public Transaction GetSingle(string id) => _transactionsService.GetTransactions(id).FirstOrDefault();

        [HttpGet("accounts")]
        public IEnumerable<TransactionAccount> GetAccounts() => _transactionsService.GetAccounts();

        [HttpPost("account")]
        public IActionResult SaveAccount([FromBody] TransactionAccount account)
        {
            _transactionsService.SaveAccount(account);
            return Ok();
        }

    }
}
