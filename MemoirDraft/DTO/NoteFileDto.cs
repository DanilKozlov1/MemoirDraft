using MemoirDraft.Database.DTO;

namespace MemoirDraft.DTO
{
    public class NoteFileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int NoteTypeId { get; set; }
        public List<TodoItem>? TodoItems { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}