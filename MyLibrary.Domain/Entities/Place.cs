using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Domain.Entities
{
    public class Place
    {
        public int PlaceID { get; set; }

        [Required(ErrorMessage = "Название не может быть пустым")]
        public string Name { get; set; } = string.Empty;

        // Коллекция для связи «один ко многим»
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
