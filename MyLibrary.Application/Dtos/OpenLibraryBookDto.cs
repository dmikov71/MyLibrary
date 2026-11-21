using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyLibrary.Application.Dtos
{
    /// <summary>
    /// DTO, описывающий структуру ответа Open Library (jscmd=data).
    /// </summary>
    public class OpenLibraryBookDto
    {
        [JsonPropertyName("publishers")]
        public List<OpenLibraryPublisherDto>? Publishers { get; set; }

        [JsonPropertyName("publish_date")]
        public string? PublishDate { get; set; }

        [JsonPropertyName("number_of_pages")]
        public int? NumberOfPages { get; set; }

        [JsonPropertyName("cover")]
        public OpenLibraryCoverDto? Cover { get; set; }

        [JsonPropertyName("identifiers")]
        public Dictionary<string, List<string>>? Identifiers { get; set; }

        // Хелпер: извлекает год из строки вида "October 1, 1988" или "1988"
        public int? GetPublishYear()
        {
            if (string.IsNullOrEmpty(PublishDate))
                return null;

            // Ищем первое четырёхзначное число, похожее на год
            var match = System.Text.RegularExpressions.Regex
                .Match(PublishDate, @"\b(1[5-9]\d{2}|20\d{2})\b");

            return match.Success ? int.Parse(match.Value) : null;
        }
    }

}
