using BusinessLogic.Ports;
using Infrastructure.Dto;
using Microsoft.AspNetCore.Mvc;
using Response = Domain.CustomEntities.Response;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [AuthorizeFilter]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Response))]
public class MarvelController(IFactoryLogic factoryLogic) : ControllerBase
{
    private readonly IMarvelService _marvelService = factoryLogic.MarvelService;

    /// <summary>
    /// Obtiene la lista de comics.
    /// </summary>
    [HttpGet("comics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MarvelComicDto>))]
    public async Task<IActionResult> GetAllComics()
    {
        var comics = await _marvelService.GetComicsAsync();
        return Ok(comics);
    }

    /// <summary>
    /// Obtiene un comic de Marvel por ID.
    /// </summary>
    [HttpGet("comics/{comicId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarvelComicDto))]
    public async Task<IActionResult> GetComicById(int comicId)
    {
        var comic = await _marvelService.GetComicByIdAsync(comicId);
        return comic != null ? Ok(comic) : NotFound(new Response { Message = "Comic no encontrado" });
    }
}