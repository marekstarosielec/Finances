using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IDocumentService _documentService;

        public DocumentsController(ILogger<DocumentsController> logger, IDocumentService documentService)
        {
            _logger = logger;
            _documentService = documentService;
        }

        [HttpGet]
        public IEnumerable<Document> Get() => _documentService.GetDocuments();

        [HttpGet("number")]
        public int GetMaxNumber() => _documentService.GetMaxDocumentNumber();

        [HttpGet("{id}")]
        public Document GetSingle(string id) => _documentService.GetDocuments(id).FirstOrDefault();

        [HttpPost("document")]
        public IActionResult Add([FromBody] Document document)
        {
            _documentService.SaveDocument(document);
            return Ok();
        }

        [HttpPut("document")]
        public IActionResult Save([FromBody] Document document)
        {
            _documentService.SaveDocument(document);
            return Ok();
        }

        [HttpDelete("document/{id}")]
        public IActionResult Delete(string id)
        {
            _documentService.DeleteDocument(id);
            return Ok();
        }
    }
}
