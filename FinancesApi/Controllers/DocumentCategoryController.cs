using FinancesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentCategoryController : ControllerBase
    {
        private readonly IDocumentCategoryService _documentCategoryService;

        public DocumentCategoryController(IDocumentCategoryService documentCategoryService)
        {
            _documentCategoryService = documentCategoryService;
        }

        [HttpGet]
        public IEnumerable<DocumentCategory> Get() => _documentCategoryService.GetDocumentCategory();

        [HttpPost("documentcategory")]
        public IActionResult Add([FromBody] DocumentCategory documentCategory)
        {
            _documentCategoryService.SaveDocumentCategory(documentCategory);
            return Ok();
        }

        [HttpPut("documentcategory")]
        public IActionResult Save([FromBody] DocumentCategory documentCategory)
        {
            _documentCategoryService.SaveDocumentCategory(documentCategory);
            return Ok();
        }

        [HttpDelete("documentcategory/{id}")]
        public IActionResult Delete(string id)
        {
            _documentCategoryService.DeleteDocumentCategory(id);
            return Ok();
        }
    }
}
