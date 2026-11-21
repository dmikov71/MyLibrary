
namespace MyLibrary.Application.Dtos
{
    public class BookLibResult
    {
        public int? Year { get; set; }
        public int? Pages { get; set; }
        public string? PublisherName { get; set; }
        public string? CoverUrl { get; set; } // URL обложки от OpenLibrary
    }
}
