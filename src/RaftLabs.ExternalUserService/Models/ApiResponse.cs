using System.Text.Json.Serialization;

namespace RaftLabs.ExternalUserService.Models
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public Support? Support { get; set; }
    }

  

    public class Support
    {
        public string Url { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}