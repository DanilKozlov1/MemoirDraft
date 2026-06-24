namespace MemoirDraft.DTO
{
    public class NoteViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int NoteTypeId { get; set; }
        public List<TodoItemDto> TodoItems { get; set; } = new();
    }
}