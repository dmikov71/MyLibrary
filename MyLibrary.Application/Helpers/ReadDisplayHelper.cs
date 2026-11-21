
using MyLibrary.Domain.Enums;

namespace MyLibrary.Application.Helpers
{
    public static class ReadDisplayHelper
    {
        public static string GetBookReadText(Read read) => read switch
        {
            Read.NoRead => "Не прочитана",
            Read.Read => "Прочитана",
            Read.WantRead => "Хочу прочитать",
            Read.WantRepeat => "Хочу перечитать",
            _ => ""
        };
    }
}
