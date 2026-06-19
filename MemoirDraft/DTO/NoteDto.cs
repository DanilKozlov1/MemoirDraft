namespace MemoirDraft.DTO
{
    public class NoteDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string NoteTypeName { get; set; } = string.Empty;
    }
}