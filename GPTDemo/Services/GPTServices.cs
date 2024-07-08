using GPTDemo.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System;
using System.Linq.Expressions;
using System.Net.WebSockets;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_ids"></param>
        /// <param name="message"></param>
        /// <param name="role"></param> == user || assitant || ....
        public record Threads(string role, List<string> messages, List<string>? file_ids);
        public async Task<string> Completions(List<Threads> messages)
        {

            var requestBody = new List<GPTCompletionsWithFilesModel>();
            if (messages is not null && messages.Count > 0) {
                List<Tools> tools = new List<Tools>();
                tools.Add(new Tools { type = "file_search" });
                foreach (var item in messages)
                {
                    List<Attachments> attachments = new List<Attachments>();
                    if ( item.file_ids is not null && item.file_ids.Count > 0)
                    {
                        item.file_ids.ForEach(x => 
                        {
                            attachments.Add(new Attachments { tools = tools, file_id = x });
                        });
                    }
                    List<Contents> contents = new List<Contents>();
                    if(item.messages is not null && item.messages.Count > 0)
                    {
                        item.messages.ForEach(x =>
                        {
                            contents.Add(new Contents { type = "text", text = x });
                        });
                    }
                    //contents.Add(new Contents { type = "text", text = item.message});
                    List<Messages> Messages = new List<Messages>();
                    Messages.Add(new Messages { role = item.role is not null ? item.role : "user", content = contents, attachments = attachments });
                    var input = JsonConvert.SerializeObject(Messages);

                    requestBody.Add(new GPTCompletionsWithFilesModel{ model = _GPTSettings.Value.Model, messages = Messages });
                }
            }
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

        public async Task<string> UploadToGptApi(string filePath, string typeFile)
        {
            var url = _GPTSettings.Value.Url + "/files";
            string TypeFile = contentType(typeFile);
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
        static string contentType(string type)
        {
            switch (type)
            {
                case ".c":
                    return "text/x-c";
                case ".cs":
                    return "text/x-csharp";
                case ".cpp":
                    return "text/x-c++";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".html":
                    return "text/html";
                case ".java":
                    return "text/x-java";
                case ".json":
                    return "application/json";
                case ".md":
                    return "text/markdown";
                case ".pdf":
                    return "application/pdf";
                case ".php":
                    return "text/x-php";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".py":
                    return "text/x-python";
                case ".s-py": // cái này là .py nhung Đạt fake thành s-py để tránh lỗi case
                    return "text/x-script.python";
                case ".rb":
                    return "text/x-ruby";
                case ".tex":
                    return "text/x-tex";
                case ".txt":
                    return "text/plain";
                case ".css":
                    return "text/css";
                case ".js":
                    return "text/javascript";
                case ".sh":
                    return "application/x-sh";
                case ".ts":
                    return "application/typescript";
                default:
                    throw new Exception("File không nằm trong danh sách hỗ trợ");
                    
            }
        }

    }
}