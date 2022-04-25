using System;
using System.Threading.Tasks;

namespace Library
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new LibraryService();
            await service.EnsureCreatedAsync();
            await service.FillRandomDataAsync();

            await service.AddAuthor(new Model.Author()
            {
                FullName = "Иванов Иван Иванович"
            });


            Console.WriteLine("GetBooksWithPublishers");
            var booksWithPublishers = await service.GetBooksWithPublishers();
            foreach (var book in booksWithPublishers)
            {
                Console.WriteLine($"Book: {book.BookId,-2} {book.BookName,-30}  Publisher: {book.PublisherId,-2} {book.PublisherName,-30}");
            }


            Console.WriteLine("GetTopAuthors(4)");
            var topAuthors = await service.GetTopAuthors(4);
            foreach (var author in topAuthors)
            {
                Console.WriteLine($"Author: {author.FullName,-30} Count: {author.Count,-2}");
            }


            Console.WriteLine("GetTopCoauthors");
            var coautors = await service.GetTopCoauthors();
            foreach (var author in coautors)
            {
                Console.WriteLine($"Author1: {author.AuthorId1,-30} Author2: {author.AuthorId2,-30} Count: {author.Count,-2}");
            }
        }
    }
}
