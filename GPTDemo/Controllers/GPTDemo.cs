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

        [HttpPost("CompletionGPT")]
        public async Task<string> GetCompletion(string file_id, string text)
        {
            string data = await _GPTservices.CompletionsWithFiles(file_id, text);

            return data;
        }
        [HttpPost("upload")]
        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file uploaded.";
            }

            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
 
        }
        [HttpPost("uploadToGPT")]
        public async Task<IActionResult> UploadFiletoGPT(string filePath)
        {
            var responseContent = await _GPTservices.UploadToGptApi(filePath);

            return Ok(responseContent);
        }
    }
}
