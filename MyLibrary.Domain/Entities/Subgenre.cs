using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Domain.Entities
{
    public class Subgenre
    {
        public int SubgenreID { get; set; }

        [Required(ErrorMessage = "Название поджанра не может быть пустым")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int GenreID { get; set; }

        // Навигационное свойство
        public Genre? Genre { get; set; }

        // Коллекция для связи «один ко многим»
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
