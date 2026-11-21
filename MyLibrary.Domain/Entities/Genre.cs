
namespace MyLibrary.Domain.Entities
{
    public class Genre
    {
        public int GenreID { get; set; }

        public string Name { get; set; } = string.Empty;

        // Коллекция для связи «один ко многим»
        public ICollection<Subgenre> Subgenres { get; set; } = new List<Subgenre>();

        // Внешний ключ к категории
        public int CategoryID { get; set; }

        public Category? Category { get; set; }

        // Коллекция для связи «один ко многим»
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
