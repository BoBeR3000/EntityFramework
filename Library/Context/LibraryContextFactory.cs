namespace Library
{
    public static class LibraryContextFactory
    {
        public static string DbFullName { get; } = "Library.db";


        public static LibraryContext CreateLibraryContext()
        {
            return new LibraryContext(DbFullName);
        }
    }
}
