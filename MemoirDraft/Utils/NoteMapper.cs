using MemoirDraft.Database.Models;
using MemoirDraft.DTO;

namespace MemoirDraft.Utils
{
    public static class NoteMapper
    {
        public static NoteDto ToDto(this Note note)
        {
            return new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                NoteTypeName = note.NoteType?.Name ?? "Без типа",
                IsFavorite = note.IsFavorite
            };
        }

        public static List<NoteDto> ToDtos(this IEnumerable<Note> notes)
        {
            return notes?.Select(n => n.ToDto()).ToList() ?? new List<NoteDto>();
        }
    }
}