using BusinessLogic.Ports;
using Microsoft.AspNetCore.Mvc;
using Response = Domain.CustomEntities.Response;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[AuthorizeFilter] //Decorador para activar la autorización
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Response))]
public class FavoriteComicsController(IFavoriteComicsService favoriteComicService) : ControllerBase
{
    /// <summary>
    /// Agrega a la lista el comic favorito.
    /// </summary>
    [HttpPost("{userId}/favorites/{comicId}")]
    public async Task<IActionResult> AddFavorite(int userId, int comicId)
    {
        await favoriteComicService.AddFavoriteComic(userId, comicId);
        return Ok(new { message = "Comic agregado a favoritos" });
    }

    /// <summary>
    /// Elimina de la lista el comic favorito.
    /// </summary>
    [HttpDelete("{userId}/favorites/{comicId}")]
    public async Task<IActionResult> RemoveFavorite(int userId, int comicId)
    {
        await favoriteComicService.RemoveFavoriteComic(userId, comicId);
        return Ok(new { message = "Comic eliminado de favoritos" });
    }

    /// <summary>
    /// Obtiene la lista los comics favoritos por Usuario.
    /// </summary>
    [HttpGet("{userId}/favorites")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        var favorites = await favoriteComicService.GetFavoriteComics(userId);
        if (!favorites.Any())
            return NotFound(new { message = "El usuario no tiene cómics favoritos." });
        return Ok(favorites);
    }
}