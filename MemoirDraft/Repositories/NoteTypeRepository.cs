using MemoirDraft.Database;
using MemoirDraft.Database.Models;
using MemoirDraft.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemoirDraft.Repositories
{
    public class NoteTypeRepository : INoteTypeRepository
    {
        private readonly AppDBContext _context;


        public NoteTypeRepository(AppDBContext context)
        {
            _context = context;
        }


        public async Task<NoteType?> GetByIdAsync(int id) => await _context.NoteTypes.FindAsync(id);

        public async Task<NoteType?> GetByNameAsync(string name) 
            => await _context.NoteTypes.FirstOrDefaultAsync(nt => nt.Name == name);

        public async Task<List<NoteType>> GetAllAsync() => await _context.NoteTypes.ToListAsync();
    }
}