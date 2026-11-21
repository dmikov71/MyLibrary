using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibrary.Domain.Entities
{
    public class Author
    {
        public int AuthorID { get; set; }

        [Required(ErrorMessage = "Фамилия автора не может быть пустой")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Имя автора не может быть пустым")]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        [NotMapped]
        public string DisplayName =>
        string.Join(" ",
        new[] { FirstName, MiddleName, LastName }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
