using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Domain.Entities
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Название категории не может быть пустым")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Код категории обязателен")]
        [StringLength(3, ErrorMessage = "Код не должен быть длиннее 3-х символов")]
        public string Code { get; set; } = string.Empty;

        // Коллекция для связи «один ко многим»
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Genre>? Genres { get; set; }
    }
}
