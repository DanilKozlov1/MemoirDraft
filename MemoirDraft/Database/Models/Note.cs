using MemoirDraft.Database.DTO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MemoirDraft.Database.Models
{
    public class Note
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }
        public int NoteTypeId { get; set; }
        public string? Content { get; set; }
        [Column(TypeName = "jsonb")]
        public List<TodoItem>? TodoItems { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(NoteTypeId))]
        public NoteType? NoteType { get; set; }
    }
}