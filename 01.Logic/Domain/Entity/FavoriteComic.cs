namespace Domain.Entity;

public class FavoriteComic
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public string ComicTitle { get; set; } = string.Empty;
    public string? ComicDescription { get; set; } = string.Empty;
    public string ComicImageUrl { get; set; } = string.Empty;
    public virtual User NaviUser { get; set; } = null!;
}