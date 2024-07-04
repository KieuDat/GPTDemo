using GPTDemo.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
namespace GPTDemo.Services
{
    public class GPTServices
    {
        private static readonly HttpClient client = new HttpClient();
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

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_GPTSettings.Value.SecretKey}");

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_GPTSettings.Value.Url + "/chat/completions", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            return jsonResponse.choices[0].text.ToString();
        }
    }
}