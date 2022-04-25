using Library.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Library
{
    public class LibraryContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<BooksWithPublisher> BooksWithPublishers { get; set; }

        public string DbFullName { get; }

        public LibraryContext()
        {
        }

        public LibraryContext(string dbFullName)
        {
            DbFullName = dbFullName;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BooksWithPublisher>(bp =>
            {
                bp.HasNoKey();
                bp.ToView("View_BooksWithPublisher");
            });
            modelBuilder.Entity<Book>().HasIndex(b => b.Id);
            modelBuilder.Entity<Author>().HasIndex(a => a.Id);
            modelBuilder.Entity<Publisher>().HasIndex(p => p.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbFullName}");
            optionsBuilder.LogTo(s =>
            {
                File.AppendAllText("Sql_log.txt", s);
                File.AppendAllText("Sql_log.txt", Environment.NewLine);
            }, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }
}
