using MyLibrary.Domain.Enums;

namespace MyLibrary.Application.Helpers
{
    public static class BookDisplayHelper
    {
        public static string GetBookTypeText(BookType type) => type switch
        {
            BookType.Printed => "Печатная",
            BookType.Electronic => "Электронная",
            _ => "Неизвестно"
        };

        public static string GetSubtypeText(ElectronicType? subtype)
        {
            if (subtype == null) return ""; 
            return subtype switch
            {
                ElectronicType.Text => "Электронный текст",
                ElectronicType.Audio => "Аудиокнига",
                _ => subtype.ToString()
            };
        }
    }

}
