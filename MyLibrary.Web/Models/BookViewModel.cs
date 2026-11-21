
namespace MyLibrary.Web.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AuthorsDisplay { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? GenreName { get; set; }
        public string? SubgenreName { get; set; }
        public string? PlaceName { get; set; }
        public int Volumes { get; set; }
        public int LibraryNumber { get; set; }
        public string DisplayLibraryNumber { get; set; } = string.Empty; // общий номер
        public string CategoryCode { get; set; } = string.Empty;   // код категории
        public string? BookTypeDisplay { get; set; }   
        public string? SubtypeDisplay { get; set; }
        public string? ReadDisplay { get; set; }
        public int Raiting { get; set; }
        public string? ISBN { get; set; }
        public string? Publishing { get; set; }
        public int? Year { get; set; }
        public int? Pages { get; set; }
        public string? CoverImage { get; set; }
    }
}
