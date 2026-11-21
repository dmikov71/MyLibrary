
namespace MyLibrary.Domain.Enums
{
    public enum Read
    {
        NoRead = 1,  // не прочитано
        Read = 2,   // прочитано
        WantRead = 3, // хочу прочитать
        WantRepeat = 4, // хочу перечитать
    }
}
