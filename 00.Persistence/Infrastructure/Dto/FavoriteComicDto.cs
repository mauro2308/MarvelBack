namespace Infrastructure.Dto;

public class FavoriteComicDto
{
    public int ComicId { get; set; }
    public string ComicTitle { get; set; } = string.Empty;
    public string? ComicDescription { get; set; } = string.Empty;
    public string ComicImageUrl { get; set; } = string.Empty;
}