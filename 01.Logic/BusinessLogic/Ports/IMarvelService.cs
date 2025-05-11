using Domain.CustomEntities;
using Infrastructure.Dto;

namespace BusinessLogic.Ports;

public interface IMarvelService
{
    Task<IEnumerable<MarvelComicDto>> GetComicsAsync();
    Task<MarvelComicDto?> GetComicByIdAsync(int comicId);
}