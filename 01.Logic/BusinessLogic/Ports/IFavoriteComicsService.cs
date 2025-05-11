using Infrastructure.Dto;

namespace BusinessLogic.Ports;

public interface IFavoriteComicsService
{
    Task AddFavoriteComic(int userId, int comicId);
    Task RemoveFavoriteComic(int userId, int comicId);
    Task<IEnumerable<FavoriteComicDto>> GetFavoriteComics(int userId);
}