using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly ICurrencyExchangeService _currencyExchangeService;

        public CurrencyExchangeController(ICurrencyExchangeService currencyExchangeService)
        {
            _currencyExchangeService = currencyExchangeService;
        }

        [HttpGet]
        public IEnumerable<CurrencyExchange> Get() => _currencyExchangeService.Get();

        [HttpPost("currency-exchange")]
        public IActionResult Add([FromBody] CurrencyExchange currencyExchange)
        {
            _currencyExchangeService.Save(currencyExchange);
            return Ok();
        }

        [HttpPut("currency-exchange")]
        public IActionResult Save([FromBody] CurrencyExchange currencyExchange)
        {
            _currencyExchangeService.Save(currencyExchange);
            return Ok();
        }

        [HttpDelete("currency-exchange/{id}")]
        public IActionResult Delete(string id)
        {
            _currencyExchangeService.Delete(id);
            return Ok();
        }
    }
}
