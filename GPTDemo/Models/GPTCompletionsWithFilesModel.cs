using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace GPTDemo.Models
{
    public class GPTCompletionsModel
    {
        public string? model { get; set; }
        public List<Messages>? messages { get; set; }
    }
    public class Messages
    {
        public string? role { get; set; }
        public List<Contents>? content { get; set; }
        public List<Attachments>? attachments { get; set; }
    }
    public class Contents
    {
        private string DefaultType = "text"; 
        public string? type { get => DefaultType; set => value = DefaultType; }
        public string? text { get; set; }
    }

    public class Attachments
    { 
        public string? file_id { get; set; }
        public List<Tools>? tools { get; set; }
    }
    public class Tools
    {
        public string? type { get; set; }
    }
    public class Optional 
    {
        public double? temperature { get; set; }
        public double? max_token { get; set; }
        public double? top_p { get; set; }
        public double? frequency_penalty { get; set; }
        public double? presence_penalty { get; set; }
    }
}
