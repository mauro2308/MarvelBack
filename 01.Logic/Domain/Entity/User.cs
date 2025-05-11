using System.Net.Mail;

namespace Domain.Entity;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Identification { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public virtual ICollection<FavoriteComic> FavoriteComics { get; set; } = null!;
}