using MyLibrary.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Web.Models
{
    public class BookFormDto
    {
        public int BookID { get; set; }

        [Required(ErrorMessage = "Название книги не может быть пустым")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите автора")]
        public List<int> SelectedAuthorIds { get; set; } = new();

        [Required(ErrorMessage = "Выберите категорию")]
        public int? CategoryID { get; set; }

        [Required(ErrorMessage = "Выберите жанр")]
        public int? GenreID { get; set; }
        public int? SubgenreID { get; set; }

        [Required(ErrorMessage = "Выберите место хранения")]
        public int? PlaceID { get; set; }
        public string? ISBN { get; set; }
        public int Volumes { get; set; }

        [Required(ErrorMessage = "Выберите вид книги")]
        public BookType Type { get; set; }

        public ElectronicType? Subtype { get; set; }
        public Read Read { get; set; }

        [Range(0, 5, ErrorMessage = "Рейтинг должен быть от 0 до 5")]
        public int Raiting { get; set; }

    }
}
