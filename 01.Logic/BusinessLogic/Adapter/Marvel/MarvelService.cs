using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BusinessLogic.Ports;
using Infrastructure.Dto;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Adapter.Marvel;

public class MarvelService(HttpClient httpClient, IConfiguration configuration) : IMarvelService
{
    private readonly string _baseUrl = configuration["MarvelApi:BaseUrl"]!;
    private readonly string _privateKey = configuration["MarvelApi:PrivateKey"]!;
    private readonly string _publicKey = configuration["MarvelApi:PublicKey"]!;

    public async Task<IEnumerable<MarvelComicDto>> GetComicsAsync()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var hash = GenerateHash(timestamp);

        var url = $"{_baseUrl}/comics?ts={timestamp}&apikey={_publicKey}&hash={hash}";

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(json);

        return jsonDocument.RootElement
            .GetProperty("data")
            .GetProperty("results")
            .EnumerateArray()
            .Select(c => new MarvelComicDto
            {
                ComicId = c.GetProperty("id").GetInt32(),
                ComicTitle = c.GetProperty("title").GetString() ?? "Sin Título",
                ComicDescription = c.GetProperty("description").GetString() ?? "Sin Descripción",
                ComicImageUrl =
                    $"{c.GetProperty("thumbnail").GetProperty("path").GetString()}.{c.GetProperty("thumbnail").GetProperty("extension").GetString()}"
                        .Replace("http://", "https://")
            })
            .ToList();
    }

    public async Task<MarvelComicDto?> GetComicByIdAsync(int comicId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var hash = GenerateHash(timestamp);
        var url = $"{_baseUrl}/comics/{comicId}?ts={timestamp}&apikey={_publicKey}&hash={hash}";

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(json);

        var comicElement = jsonDocument.RootElement
            .GetProperty("data")
            .GetProperty("results")
            .EnumerateArray()
            .FirstOrDefault();

        if (comicElement.ValueKind == JsonValueKind.Undefined)
            return null;

        return new MarvelComicDto
        {
            ComicId = comicElement.GetProperty("id").GetInt32(),
            ComicTitle = comicElement.GetProperty("title").GetString() ?? "Sin Título",
            ComicDescription = comicElement.GetProperty("description").GetString() ?? "Sin Descripción",
            ComicImageUrl =
                $"{comicElement.GetProperty("thumbnail").GetProperty("path").GetString()}.{comicElement.GetProperty("thumbnail").GetProperty("extension").GetString()}"
                    .Replace("http://", "https://")
        };
    }

    private string GenerateHash(string timestamp)
    {
        var input = $"{timestamp}{_privateKey}{_publicKey}";
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}