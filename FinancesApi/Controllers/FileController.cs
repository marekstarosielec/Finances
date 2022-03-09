using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public DecompressFileResult DecompressFile([FromBody] DecompressFileInstruction instruction) => new DecompressFileResult { Path = _fileService.DecompressFile(instruction.Number, instruction.Password) };
    }
}
