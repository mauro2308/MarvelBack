namespace Infrastructure.Dto;

public class MarvelComicDto
{
    public int ComicId { get; set; }
    public string ComicTitle { get; set; } = "Sin Título";
    public string ComicDescription { get; set; } = "Sin Descripción";
    public string ComicImageUrl { get; set; } = string.Empty;
}