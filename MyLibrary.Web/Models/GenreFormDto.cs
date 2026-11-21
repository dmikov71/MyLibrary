using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Web.Models
{
    public class GenreFormDto
    {
        public int? GenreID { get; set; } 

        [Required(ErrorMessage = "Название жанра не может быть пустым")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выбор категории обязателен")]
        public int? CategoryID { get; set; }
    }
}
