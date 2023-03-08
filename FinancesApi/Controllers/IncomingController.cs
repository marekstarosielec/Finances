using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncomingController : ControllerBase
    {
        private readonly IIncomingService _incomingService;
        private readonly IDocumentService _documentService;

        public IncomingController(IIncomingService incomingService, IDocumentService documentService)
        {
            _incomingService = incomingService;
            _documentService = documentService;
        }
        
        [HttpGet]
        public IEnumerable<Incoming> Get() => _incomingService.Get();

        [HttpPost()]
        public string Add([FromBody] Incoming incoming)
        {
            return _documentService.ConvertFileToDocument(incoming.FullFileName);
        }

        [HttpPost("ConvertToPdf")]
        public string ConvertToPdf([FromBody] List<Incoming> incoming)
        {
            return _documentService.ConvertFilesToPdf(incoming.Select(i => i.FullFileName).ToList());
        }
    }
}
