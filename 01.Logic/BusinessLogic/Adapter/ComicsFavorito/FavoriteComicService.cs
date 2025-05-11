using System.Net;
using BusinessLogic.Ports;
using Domain.Entity;
using Domain.Exceptions;
using Domain.Interface;
using Infrastructure.Dto;

namespace BusinessLogic.Adapter.ComicsFavorito;

public class FavoriteComicService(
    IFavoriteComicsRepository favoriteComicRepository,
    IUserRepository userRepository,
    IMarvelService marvelService)
    : IFavoriteComicsService
{
    public async Task AddFavoriteComic(int userId, int comicId)
    {
        var comic = await marvelService.GetComicByIdAsync(comicId);
        if (comic == null)
            throw new BusinessException(HttpStatusCode.BadRequest, "Error id comic no válido",
                $"No se encontró el cómic con ID {comicId} en la API de Marvel.");

        var user = userRepository.Detail(userId);
        if (user == null)
            throw new BusinessException(HttpStatusCode.NotFound, "Error id usuario no encontrado",
                "Id usuario no válido");

        var favoriteComic = new FavoriteComic
        {
            UserId = userId,
            ComicId = comic.ComicId,
            ComicTitle = comic.ComicTitle,
            ComicDescription = comic.ComicDescription,
            ComicImageUrl = comic.ComicImageUrl
        };

        await favoriteComicRepository.AddFavoriteComic(favoriteComic);
    }

    public async Task RemoveFavoriteComic(int userId, int comicId)
    {
        var comic = await marvelService.GetComicByIdAsync(comicId);
        if (comic == null)
            throw new BusinessException(HttpStatusCode.BadRequest, "Error id comic no válido",
                $"No se encontró el cómic con ID {comicId} en la API de Marvel.");

        var user = userRepository.Detail(userId);
        if (user == null)
            throw new BusinessException(HttpStatusCode.BadRequest, "Error usuario no válido",
                "Usuario no válido");

        await favoriteComicRepository.RemoveFavoriteComic(userId, comicId);
    }

    public async Task<IEnumerable<FavoriteComicDto>> GetFavoriteComics(int userId)
    {
        var favorites = await favoriteComicRepository.GetFavoriteComicsByUserId(userId);

        return favorites.Select(fc => new FavoriteComicDto
        {
            ComicId = fc.ComicId,
            ComicTitle = fc.ComicTitle,
            ComicDescription = fc.ComicDescription,
            ComicImageUrl = fc.ComicImageUrl
        }).ToList();
    }
}