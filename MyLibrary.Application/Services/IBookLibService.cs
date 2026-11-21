
using MyLibrary.Application.Dtos;

namespace MyLibrary.Application.Services
{
    public interface IBookLibService
    {
        Task<BookLibResult?> FetchByIsbnAsync(string isbn);
    }
}
