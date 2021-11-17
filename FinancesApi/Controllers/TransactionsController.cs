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

        [HttpPost("transaction")]
        public IActionResult Add([FromBody] Transaction transaction)
        {
            _transactionsService.SaveTransaction(transaction);
            return Ok();
        }

        [HttpPost("autocategorize")]
        public IActionResult AutoCategorize()
        {
            _transactionsService.ApplyAutoCategories();
            return Ok();
        }

        [HttpPut("transaction")]
        public IActionResult Save([FromBody] Transaction transaction)
        {
            _transactionsService.SaveTransaction(transaction);
            return Ok();
        }

        [HttpDelete("transaction/{id}")]
        public IActionResult Delete(string id)
        {
            _transactionsService.DeleteTransaction(id);
            return Ok();
        }

        [HttpGet("accounts")]
        public IEnumerable<TransactionAccount> GetAccounts() => _transactionsService.GetAccounts();

        [HttpPost("account")]
        public IActionResult AddAccount([FromBody] TransactionAccount account)
        {
            _transactionsService.SaveAccount(account);
            return Ok();
        }

        [HttpPut("account")]
        public IActionResult SaveAccount([FromBody] TransactionAccount account)
        {
            _transactionsService.SaveAccount(account);
            return Ok();
        }

        [HttpDelete("account/{id}")]
        public IActionResult DeleteAccount(string id)
        {
            _transactionsService.DeleteAccount(id);
            return Ok();
        }

        [HttpGet("categories")]
        public IEnumerable<TransactionCategory> GetCategories() => _transactionsService.GetCategories();

        [HttpPost("category")]
        public IActionResult AddCategory([FromBody] TransactionCategory category)
        {
            _transactionsService.SaveCategory(category);
            return Ok();
        }

        [HttpPut("category")]
        public IActionResult SaveCategory([FromBody] TransactionCategory category)
        {
            _transactionsService.SaveCategory(category);
            return Ok();
        }

        [HttpDelete("category/{id}")]
        public IActionResult DeleteCategory(string id)
        {
            _transactionsService.DeleteCategory(id);
            return Ok();
        }

        [HttpGet("autocategories")]
        public IEnumerable<TransactionAutoCategory> GetAutoCategories() => _transactionsService.GetAutoCategories();

        [HttpPost("autocategory")]
        public IActionResult AddAutoCategory([FromBody] TransactionAutoCategory category)
        {
            _transactionsService.SaveAutoCategory(category);
            return Ok();
        }

        [HttpPut("autocategory")]
        public IActionResult SaveAutoCategory([FromBody] TransactionAutoCategory category)
        {
            _transactionsService.SaveAutoCategory(category);
            return Ok();
        }

        [HttpDelete("autocategory/{id}")]
        public IActionResult DeleteAutoCategory(string id)
        {
            _transactionsService.DeleteAutoCategory(id);
            return Ok();
        }
    }
}
