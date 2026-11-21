using MyLibrary.Domain.Enums;

namespace MyLibrary.Domain.Entities
{
    public class Book
    {
        public int BookID { get; set; }
        public int LibraryNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Publishing { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }
        public int Volumes { get; set; }
        public string? ISBN { get; set; }
        public Read Read{ get; set; } 
        public BookType Type { get; set; }
        public ElectronicType? Subtype { get; set; }
        public int Raiting { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public int CategoryID { get; set; }
        public Category? Category { get; set; }
        public int GenreID { get; set; }
        public Genre? Genre{ get; set; }
        public int? SubgenreID { get; set; }
        public Subgenre? Subgenre { get; set; }
        public int PlaceID { get; set; }
        public Place? Place { get; set; }
        public string? CoverImage { get; set; }

    }
}
