using MemoirDraft.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoirDraft.Database
{
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<NoteType> NoteTypes { get; set; }
        public DbSet<Note> Notes { get; set; }


        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Note>()
                .Property(n => n.TodoItems)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'[]'::jsonb");

            modelBuilder.Entity<Note>()
                .HasIndex(n => new { n.UserId, n.CreatedAt });

            modelBuilder.Entity<Note>()
                .HasIndex(n => n.IsFavorite);

            // Тестовый пользователь
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Nothing",
                    Password = "123"
                }    
            );
            // Типы заметок
            modelBuilder.Entity<NoteType>().HasData(
                new NoteType { Id = 1, Name = "simple" },
                new NoteType { Id = 2, Name = "todo" }
            );
        }
    }
}