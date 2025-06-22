using System.Text.Json.Serialization;

namespace RaftLabs.ExternalUserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;

        // Computed property for full name
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}