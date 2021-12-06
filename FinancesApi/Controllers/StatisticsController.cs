using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(ILogger<StatisticsController> logger, IStatisticsService statisticsService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public StatisticsAll Get() => _statisticsService.GetAll();

        [HttpGet("bills")]
        public StatisticsBills GetBills() => _statisticsService.GetBills();



    }
}
