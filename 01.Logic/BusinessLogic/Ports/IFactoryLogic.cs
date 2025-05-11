using Domain.Interface;

namespace BusinessLogic.Ports;

public interface IFactoryLogic
{
    //Services
    IUserService UsuarioService { get; set; }
    IMarvelService MarvelService { get; set; }
    IFavoriteComicsService FavoriteComicsService { get; set; }

    //Repositories
    IUserRepository UsuarioRepository { get; set; }
    IFavoriteComicsRepository FavoriteComicsRepository { get; set; }
}
