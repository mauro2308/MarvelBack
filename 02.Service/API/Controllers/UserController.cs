using BusinessLogic.Adapter.Usuario;
using BusinessLogic.Ports;
using Infrastructure.Dto;
using Microsoft.AspNetCore.Mvc;
using Response = Domain.CustomEntities.Response;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[AuthorizeFilter] //Decorador para activar la autorización
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Response))]
public class UserController(IFactoryLogic factoryLogic) : ControllerBase
{
    private readonly IUserService _usuarioService = factoryLogic.UsuarioService;

    /// <summary>
    ///     Obtiene la lista de usuarios.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    public async Task<IActionResult> All()
    {
        var user = await _usuarioService.All();
        return Ok(user);
    }

    /// <summary>
    ///     Creación de usuario.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
    public async Task<IActionResult> Create([FromBody] UserDto row)
    {
        return Ok(await _usuarioService.Create(row));
    }

    /// <summary>
    ///     Actualización de usuario.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
    public async Task<IActionResult> Update([FromBody] UserDto row)
    {
        return Ok(await _usuarioService.Update(row));
    }

    /// <summary>
    ///     Eliminar usuario.
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    public async Task<IActionResult> Delete(int userId)
    {
        return Ok(await _usuarioService.Delete(userId));
    }

    /// <summary>
    ///     Login de usuario
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        var token = await _usuarioService.Login(loginRequest.Email, loginRequest.Password);
        return Ok(new { token });
    }
}