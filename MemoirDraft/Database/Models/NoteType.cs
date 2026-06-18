namespace MemoirDraft.Database.Models
{
    public class NoteType
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}