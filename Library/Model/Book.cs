using System.Collections.Generic;

namespace Library.Model
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Publisher Publisher { get; set; }

        public int Count { get; set; }

        public List<Author> Authors { get; set; } = new List<Author>();
    }
}
