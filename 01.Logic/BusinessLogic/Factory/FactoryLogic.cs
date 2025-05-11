using BusinessLogic.Adapter.ComicsFavorito;
using BusinessLogic.Adapter.Marvel;
using BusinessLogic.Adapter.Usuario;
using BusinessLogic.Ports;
using Domain.Interface;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Repository.ComicFavorito;
using Repository.Usuario;

namespace BusinessLogic.Factory;

// Esta clase aplica los patrones Factory y Dependency Injection (DI).
// Factory: Centraliza la creación de instancias de repositorios y servicios.
// DI: Recibe las dependencias IUnitOfWork y IMapper desde afuera, facilitando la inyección de dependencias.
public class FactoryLogic : IFactoryLogic
{
    public FactoryLogic(
        IUnitOfWork unitOfWork,
        IMarvelService marvelService,
        IFavoriteComicsRepository favoriteComicRepository,
        IUserRepository userRepository,
        IConfiguration configuration
    )
    {
        // Repositorios
        UsuarioRepository = new UserRepository(unitOfWork);
        FavoriteComicsRepository = favoriteComicRepository;

        // Servicios
        UsuarioService = new UserService(UsuarioRepository, configuration);
        MarvelService = marvelService;
        FavoriteComicsService = new FavoriteComicService(FavoriteComicsRepository, UsuarioRepository, MarvelService);
    }

    // Repositorios
    public IUserRepository UsuarioRepository { get; set; }
    public IFavoriteComicsRepository FavoriteComicsRepository { get; set; }

    // Servicios
    public IUserService UsuarioService { get; set; }
    public IFavoriteComicsService FavoriteComicsService { get; set; }
    public IMarvelService MarvelService { get; set; }
}
