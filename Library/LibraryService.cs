using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library
{
    public class LibraryService
    {
        private readonly Random _random;

        public LibraryService()
        {
            _random = new Random();
        }

        public async Task EnsureCreatedAsync()
        {
            using var context = LibraryContextFactory.CreateLibraryContext();

            // пересоздадим базу данных
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            context.Database.ExecuteSqlRaw(
                    @"CREATE VIEW 
                    View_BooksWithPublisher as 
                    SELECT b.Id AS BookId, b.Name AS BookName, p.Id AS PublisherId, p.Name AS PublisherName
                    FROM Books b
                    INNER JOIN Publishers p on b.PublisherId = p.Id
                    ORDER BY p.Id");
        }

        public async Task AddAuthor(Model.Author author)
        {
            using var context = LibraryContextFactory.CreateLibraryContext();
            await context.Authors.AddAsync(author);
            await context.SaveChangesAsync();
        }

        public async Task AddBook(Model.Book book)
        {
            using var context = LibraryContextFactory.CreateLibraryContext();
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

        }

        public async Task AddPublishing(Model.Publisher publisher)
        {
            using var context = LibraryContextFactory.CreateLibraryContext();
            await context.Publishers.AddAsync(publisher);
            await context.SaveChangesAsync();
        }

        public async Task<List<Model.BooksWithPublisher>> GetBooksWithPublishers()
        {
            using var context = LibraryContextFactory.CreateLibraryContext();
            return await context.BooksWithPublishers.ToListAsync();
        }

        public async Task<List<AuthorPublishCount>> GetTopAuthors(int publisherId)
        {
            using var context = LibraryContextFactory.CreateLibraryContext();
            var query = context.Books
                .Where(b => b.Publisher.Id == publisherId)
                .SelectMany(b => b.Authors)
                .GroupBy(a => new { a.Id, a.FullName })
                .Select((g) => new AuthorPublishCount
                {
                    AuthorId = g.Key.Id,
                    FullName = g.Key.FullName,
                    Count = g.Count()
                })
                .OrderBy(g => g.Count)
                .Take(10);
            return await query.ToListAsync();
//select count(ab.BooksId) as c, AuthorsId, a.FullName from Books b
//inner join AuthorBook ab on ab.BooksId = b.Id
//inner join Authors a on ab.AuthorsId = a.Id
//where b.PublisherId = 4
//group by ab.AuthorsId
//order by c
//limit 10
        }

        public async Task<List<Coauthors>> GetTopCoauthors()
        {
            using var context = LibraryContextFactory.CreateLibraryContext();
            var books = await context.Books
                .Include(a => a.Authors)
                .ToListAsync();

            var list = new List<Coauthors>();
            foreach (var book in books)
            {
                for (int i = 0; i< book.Authors.Count; i++ ) 
                {
                    for (int j = i+1; j < book.Authors.Count; j++)
                    {
                        var pair = list.FirstOrDefault(x => (x.AuthorId1 == book.Authors[i].Id && x.AuthorId2 == book.Authors[j].Id)
                                                        || (x.AuthorId1 == book.Authors[j].Id && x.AuthorId2 == book.Authors[i].Id));
                        if (pair == null)
                        {
                            list.Add(new Coauthors()
                            {
                                AuthorId1 = book.Authors[i].Id,
                                AuthorFullName1 = book.Authors[i].FullName,
                                AuthorId2 = book.Authors[j].Id,
                                AuthorFullName2 = book.Authors[j].FullName,
                                Count = 1
                            });
                        }
                        else
                        {
                            pair.Count++;
                        }
                    }
                }
            }
            return list.OrderBy(x => x.Count).ToList();

//SELECT count(*) as c, ab1.AuthorsId, ab2.AuthorsId from AuthorBook ab1
//INNER JOIN AuthorBook ab2 on ab1.BooksId = ab2.BooksId
//where ab1.AuthorsId > ab2.AuthorsId
//GROUP BY ab1.AuthorsId, ab2.AuthorsId
//order by c
        }

       

        public async Task FillRandomDataAsync()
        {
            using var context = LibraryContextFactory.CreateLibraryContext();
            var authors = new List<Model.Author>();
            for (int i = 0; i < 10; i++)
            {
                authors.Add(new Model.Author()
                {
                    FullName = SampleData.AuthorNames[i]
                });
            }
            var publishers = new List<Model.Publisher>();
            for (int i = 0; i < 10; i++)
            {
                publishers.Add(new Model.Publisher()
                {
                    Name = SampleData.Publishers[i]
                });
            }
            var books = new List<Model.Book>();
            for (int i = 0; i < 20; i++)
            {
                var bookAuthors = new List<Model.Author>();
                var bookAuthorCount = _random.Next(1, 4);
                for (int j = 0; j < bookAuthorCount; j++)
                {
                    bookAuthors.Add(authors[_random.Next(10)]);
                }
                books.Add(new Model.Book()
                {
                    Name = SampleData.BookNames[i],
                    Authors = bookAuthors,
                    Publisher = publishers[_random.Next(10)]
                });
            }

            await context.Authors.AddRangeAsync(authors);
            await context.Publishers.AddRangeAsync(publishers);
            await context.Books.AddRangeAsync(books);

            await context.SaveChangesAsync();
        }
    }
}

