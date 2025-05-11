using Domain.Entity;

namespace Domain.Interface;

public interface IFavoriteComicsRepository
{
    Task AddFavoriteComic(FavoriteComic comic);
    Task RemoveFavoriteComic(int userId, int comicId);
    Task<IEnumerable<FavoriteComic>> GetFavoriteComicsByUserId(int userId);
}