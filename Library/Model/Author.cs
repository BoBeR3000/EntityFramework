using System.Collections.Generic;

namespace Library.Model
{
    public class Author
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
    }
}
