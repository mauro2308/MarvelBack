using BusinessLogic.Adapter.ComicsFavorito;
using Domain.Entity;
using Domain.Exceptions;
using Domain.Interface;
using Infrastructure.Dto;
using Moq;
using FluentAssertions;
using BusinessLogic.Ports;

namespace Back.Tests
{
    public class FavoriteComicServiceTests
    {
        private readonly Mock<IFavoriteComicsRepository> _mockFavoriteComicRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMarvelService> _mockMarvelService;
        private readonly FavoriteComicService _favoriteComicService;

        public FavoriteComicServiceTests()
        {
            _mockFavoriteComicRepository = new Mock<IFavoriteComicsRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMarvelService = new Mock<IMarvelService>();

            _favoriteComicService = new FavoriteComicService(
                _mockFavoriteComicRepository.Object,
                _mockUserRepository.Object,
                _mockMarvelService.Object
            );
        }

        [Fact]
        public async Task AddFavoriteComic_ShouldAddFavorite_WhenValid()
        {
            var userId = 1;
            var comicId = 100;
            var comic = new MarvelComicDto()
            {
                ComicId = comicId,
                ComicTitle = "Spider-Man",
                ComicDescription = "El asombroso Hombre Araña",
                ComicImageUrl = "https://example.com/spiderman.jpg"
            };

            var user = new User { UserId = userId, Name = "Juan" };

            _mockMarvelService.Setup(s => s.GetComicByIdAsync(comicId)).ReturnsAsync(comic);
            _mockUserRepository.Setup(r => r.Detail(userId)).Returns(user);

            await _favoriteComicService.AddFavoriteComic(userId, comicId);

            _mockFavoriteComicRepository.Verify(r => r.AddFavoriteComic(It.Is<FavoriteComic>(
                fc => fc.UserId == userId && fc.ComicId == comicId
            )), Times.Once);
        }

        [Fact]
        public async Task AddFavoriteComic_ShouldThrowException_WhenComicNotFound()
        {
            var userId = 1;
            var comicId = 999;

            _mockMarvelService.Setup(s => s.GetComicByIdAsync(comicId)).ReturnsAsync((MarvelComicDto)null!);

            Func<Task> act = async () => await _favoriteComicService.AddFavoriteComic(userId, comicId);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("No se encontró el cómic con ID 999 en la API de Marvel.");
        }

        [Fact]
        public async Task AddFavoriteComic_ShouldThrowException_WhenUserNotFound()
        {
            var userId = 999;
            var comicId = 100;
            var comic = new MarvelComicDto
            {
                ComicId = comicId,
                ComicTitle = "Spider-Man",
                ComicDescription = "El asombroso Hombre Araña",
                ComicImageUrl = "https://example.com/spiderman.jpg"
            };

            _mockMarvelService.Setup(s => s.GetComicByIdAsync(comicId)).ReturnsAsync(comic);
            _mockUserRepository.Setup(r => r.Detail(userId)).Returns((User)null!);

            var act = async () => await _favoriteComicService.AddFavoriteComic(userId, comicId);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Id usuario no válido");
        }

        [Fact]
        public async Task RemoveFavoriteComic_ShouldRemoveFavorite_WhenValid()
        {
            var userId = 1;
            var comicId = 100;
            var comic = new MarvelComicDto
            {
                ComicId = comicId,
                ComicTitle = "Spider-Man",
                ComicDescription = "El asombroso Hombre Araña",
                ComicImageUrl = "https://example.com/spiderman.jpg"
            };

            var user = new User { UserId = userId, Name = "Juan" };

            _mockMarvelService.Setup(s => s.GetComicByIdAsync(comicId)).ReturnsAsync(comic);
            _mockUserRepository.Setup(r => r.Detail(userId)).Returns(user);

            await _favoriteComicService.RemoveFavoriteComic(userId, comicId);

            _mockFavoriteComicRepository.Verify(r => r.RemoveFavoriteComic(userId, comicId), Times.Once);
        }

        [Fact]
        public async Task RemoveFavoriteComic_ShouldThrowException_WhenComicNotFound()
        {
            var userId = 1;
            var comicId = 999;

            _mockMarvelService.Setup(s => s.GetComicByIdAsync(comicId)).ReturnsAsync((MarvelComicDto)null!);

            Func<Task> act = async () => await _favoriteComicService.RemoveFavoriteComic(userId, comicId);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("No se encontró el cómic con ID 999 en la API de Marvel.");
        }

        [Fact]
        public async Task RemoveFavoriteComic_ShouldThrowException_WhenUserNotFound()
        {
            var userId = 999;
            var comicId = 100;
            var comic = new MarvelComicDto
            {
                ComicId = comicId,
                ComicTitle = "Spider-Man",
                ComicDescription = "El asombroso Hombre Araña",
                ComicImageUrl = "https://example.com/spiderman.jpg"
            };

            _mockMarvelService.Setup(s => s.GetComicByIdAsync(comicId)).ReturnsAsync(comic);
            _mockUserRepository.Setup(r => r.Detail(userId)).Returns((User)null!);

            Func<Task> act = async () => await _favoriteComicService.RemoveFavoriteComic(userId, comicId);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Usuario no válido");
        }

        [Fact]
        public async Task GetFavoriteComics_ShouldReturnFavorites_WhenExist()
        {
            var userId = 1;
            var favorites = new List<FavoriteComic>
            {
                new()
                {
                    UserId = userId,
                    ComicId = 100,
                    ComicTitle = "Spider-Man",
                    ComicDescription = "El asombroso Hombre Araña",
                    ComicImageUrl = "https://example.com/spiderman.jpg"
                },
                new()
                {
                    UserId = userId,
                    ComicId = 101,
                    ComicTitle = "Iron Man",
                    ComicDescription = "El invencible Iron Man",
                    ComicImageUrl = "https://example.com/ironman.jpg"
                }
            };

            _mockFavoriteComicRepository.Setup(r => r.GetFavoriteComicsByUserId(userId))
                .ReturnsAsync(favorites);

            var result = await _favoriteComicService.GetFavoriteComics(userId);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().ComicTitle.Should().Be("Spider-Man");
            result.Last().ComicTitle.Should().Be("Iron Man");
        }
    }
}
