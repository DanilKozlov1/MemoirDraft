using MemoirDraft.Database.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoirDraft.Database.Models
{
    public class Note
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }
        public int NoteTypeId { get; set; }
        public required string Content { get; set; }
        [Column(TypeName = "jsonb")]
        public List<TodoItem>? TodoItems { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        [ForeignKey(nameof(NoteTypeId))]
        public NoteType? NoteType { get; set; }
    }
}