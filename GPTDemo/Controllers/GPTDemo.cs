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
    }
}
