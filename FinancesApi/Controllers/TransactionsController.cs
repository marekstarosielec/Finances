using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ILogger<TransactionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            string fileName = @"c:\Users\marek\Downloads\data.json";
            string jsonString = System.IO.File.ReadAllText(fileName);
            var data =
               JsonSerializer.Deserialize<Transaction[]>(jsonString);

            return data;
        }
    }
}
