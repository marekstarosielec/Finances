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
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<CurrenciesController> _logger;
        private readonly ICurrenciesService _currenciesService;

        public CurrenciesController(ILogger<CurrenciesController> logger, ICurrenciesService currenciesService)
        {
            _logger = logger;
            _currenciesService = currenciesService;
        }

        [HttpGet]
        public IEnumerable<Currency> Get() => _currenciesService.GetCurrencies();
    }
}
