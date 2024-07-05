using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GPTDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OpenAIGPTDemo
{
    [ApiController]
    [Route("[controller]")]
    public class GptdemoController : ControllerBase
    {
        private readonly GPTServices _GPTservices;
        public GptdemoController(GPTServices _GPTservices)
        {
            this._GPTservices = _GPTservices;
        }

        [HttpPost]
        public async Task<string> GetCompletion(string file_id, string text)
        {
            string data = await _GPTservices.CompletionsWithFiles(file_id, text);

            return data;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var responseContent = await _GPTservices.UploadToGptApi(filePath);

            return Ok(responseContent);
        }
    }
}
