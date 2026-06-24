using MemoirDraft.Database;
using MemoirDraft.Database.Models;
using MemoirDraft.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemoirDraft.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDBContext _context;


        public NoteRepository(AppDBContext context)
        {
            _context = context;
        }


        public async Task<Note?> GetByIdAsync(int id) => await _context.Notes.FindAsync(id);

        public async Task<List<Note>> GetAllByUserAsync(int userId)
        {
            return await _context.Notes
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.NoteType)
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Note>> GetAllByNoteTypeAsync(int userId, int noteTypeId)
        {
            return await _context.Notes
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.NoteType)
                .Where(n => n.UserId == userId && n.NoteTypeId == noteTypeId)
                .ToListAsync();
        }

        public async Task<List<Note>> GetFavoriteNotesAsync(int userId)
        {
            return await _context.Notes
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.NoteType)
                .Where(n => n.UserId == userId && n.IsFavorite)
                .ToListAsync();
        }

        public async Task AddAsync(Note note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Note note)
        {
            var existing = await _context.Notes.FindAsync(note.Id);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(note);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
                return false;

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}