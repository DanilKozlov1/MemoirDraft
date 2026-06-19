using System.Text.Json.Serialization;

namespace MemoirDraft.Database.DTO
{
    public class TodoItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("done")]
        public bool IsDone { get; set; }
    }
}