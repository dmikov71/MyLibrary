using System.Text.Json.Serialization;

public class OpenLibraryPublisherDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
