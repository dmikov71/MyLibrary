using MyLibrary.Application.Dtos;
using MyLibrary.Application.Helpers;
using System.Text.Json;

namespace MyLibrary.Application.Services
{
    public class BookLibService : IBookLibService
    {
        private readonly HttpClient _httpClient;

        public BookLibService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookLibResult?> FetchByIsbnAsync(string isbn)
        {
            isbn = isbn.Replace("-", "").Replace(" ", "").Trim();

            try
            {
                // Таймаут 15 секунд 
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                var response = await _httpClient.GetAsync(
                    $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data",
                    cts.Token);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync(cts.Token);
                var data = JsonSerializer.Deserialize<Dictionary<string, OpenLibraryBookDto>>(json);

                var key = $"ISBN:{isbn}";
                if (data is null || !data.TryGetValue(key, out var bookDto) || bookDto is null)
                    return null;

                return new BookLibResult
                {
                    Year = bookDto.GetPublishYear(),
                    Pages = bookDto.NumberOfPages,
                    CoverUrl = bookDto.Cover?.Medium
                    ?? bookDto.Cover?.Large
                    ?? bookDto.Cover?.Small
                    ?? "/img/no-cover.png", 

                    PublisherName = Transliterator.LatinToCyrillic(bookDto.Publishers?.FirstOrDefault()?.Name)

                };
            }
            catch (TaskCanceledException)
            {
                // Сработал таймаут
                return null;
            }
            catch (HttpRequestException)
            {
                // Сеть недоступна / соединение разорвано
                return null;
            }
            catch (JsonException)
            {
                // Ответ не разобрался
                return null;
            }
        }


    }
}
