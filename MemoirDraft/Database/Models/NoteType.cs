using System.Text.Json.Serialization;

namespace MemoirDraft.Database.Models
{
    public class NoteType
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        [JsonIgnore]
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}