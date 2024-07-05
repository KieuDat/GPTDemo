using GPTDemo.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace GPTDemo.Services
{
    public class GPTServices
    {
        private readonly HttpClient client = new HttpClient();
        private readonly IOptions<GPTSettings> _GPTSettings;
        public GPTServices(IOptions<GPTSettings> _GPTSettings)
        {
            this._GPTSettings = _GPTSettings;
        }
        public async Task<string> CompletionsWithFiles(string file_id, string text) 
        {
            List<Tools> tools = new List<Tools>();
                tools.Add(new Tools { type = "file_search" });
            List<Attachments> attachments = new List<Attachments>();
                attachments.Add(new Attachments {tools = tools, file_id = file_id});
            List<Contents> contents = new List<Contents>();
                contents.Add(new Contents { type = "text", text = text});
            List<Messages> Messages = new List<Messages>();
                Messages.Add(new Messages { role = "user", content = contents, attachments = attachments});

            var requestBody = new GPTCompletionsWithFilesModel{ model = _GPTSettings.Value.Model, messages = Messages };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _GPTSettings.Value.SecretKey);

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_GPTSettings.Value.Url + "/chat/completions", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            if (jsonResponse?.choices is not null)
            {
                return jsonResponse.choices[0].content.ToString();
            }
            else 
            {
                return jsonResponse.ToString();
            }
            
        }

        public async Task<string> UploadToGptApi(string filePath)
        {
            var url = _GPTSettings.Value.Url + "/files";

            using (var form = new MultipartFormDataContent())
            {
                var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                form.Add(fileContent, "file", Path.GetFileName(filePath));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _GPTSettings.Value.SecretKey);

                var response = await client.PostAsync(url, form);

                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }
    }
}