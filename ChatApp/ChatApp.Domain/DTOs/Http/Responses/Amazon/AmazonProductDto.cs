using Newtonsoft.Json;

namespace ChatApp.Domain.DTOs.Http.Responses.Amazon;

public record AmazonProductDto
{
    [JsonProperty("image")]
    public string ImageUrl { get; set; } = null!;
    [JsonProperty("name")]
    public string Title { get; set; } = null!;
}