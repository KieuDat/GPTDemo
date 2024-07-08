using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GPTDemo.Models;
using GPTDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static GPTDemo.Services.GPTServices;

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

        [HttpPost("completionGPT")]
        public async Task<string> GetCompletion(List<Threads> threads)
        {
                string data = await _GPTservices.Completions(threads);
                return data;
        }
        public record fileUploadResult(string filePath, string fileExt);
        [HttpPost("uploadFileGPT")]
        public async Task<fileUploadResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
               throw new Exception("No file uploaded.");
            }
            string fileExt = System.IO.Path.GetExtension(file.FileName);
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var data = new fileUploadResult(filePath = filePath, fileExt = fileExt);
            return data;

        }
        [HttpPost("uploadToGPT")]
        public async Task<IActionResult> UploadFiletoGPT(string filePath, string typeFile)
        {
            var responseContent = await _GPTservices.UploadToGptApi(filePath, typeFile);

            return Ok(responseContent);
        }
    }
}
