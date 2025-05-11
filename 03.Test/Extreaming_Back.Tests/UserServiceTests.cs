using System.IdentityModel.Tokens.Jwt;
using BusinessLogic.Adapter.Usuario;
using Domain.Entity;
using Domain.Exceptions;
using Infrastructure.Dto;
using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using Domain.Interface;



namespace Back.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("c3VwZXJzZWNyZXRrZXlmb3Jqd3RhdXRoZW50aWNhdGlvbg==");
            mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("TestIssuer");
            mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("TestAudience");

            _userService = new UserService(_mockUserRepository.Object, mockConfiguration.Object);
        }

        [Fact]
        public async Task All_ReturnsUserList()
        {
            var users = new List<User>
            {
                new() { UserId = 1, Name = "Juan", Email = "juan@example.com" },
                new() { UserId = 2, Name = "Maria", Email = "maria@example.com" }
            };
            _mockUserRepository.Setup(repo => repo.All()).ReturnsAsync(users);

            var result = await _userService.All();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Juan");
            result.Last().Name.Should().Be("Maria");
        }

        [Fact]
        public async Task Create_ThrowsException_WhenUserWithSameNameExists()
        {
            var existingUsers = new List<User>
            {
                new() { UserId = 1, Name = "Juan", Email = "juan@example.com" }
            };
            _mockUserRepository.Setup(repo => repo.All()).ReturnsAsync(existingUsers);

            var newUser = new UserDto { Name = "Juan", Email = "newjuan@example.com" };

            Func<Task> act = async () => await _userService.Create(newUser);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Ya existe un usuario con el mismo nombre.");
        }

        [Fact]
        public async Task Create_AddsUser_WhenNoDuplicateFound()
        {
            var newUser = new UserDto
            {
                Name = "Maria",
                Email = "maria@example.com",
                Identification = "1234567890",
                Password = "password123"
            };

            _mockUserRepository.Setup(repo => repo.All()).ReturnsAsync(new List<User>());

            var result = await _userService.Create(newUser);

            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("Usuario creado exitosamente");

            _mockUserRepository.Verify(repo => repo.Create(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsToken_WhenCredentialsAreValid()
        {
            var user = new User
            {
                UserId = 1,
                Name = "Juan",
                Email = "juan@example.com",
                Password = "password123"
            };

            _mockUserRepository.Setup(repo => repo.All()).ReturnsAsync(new List<User> { user });

            var token = await _userService.Login("juan@example.com", "password123");

            token.Should().NotBeNullOrEmpty();

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token);
            securityToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value.Should().Be("juan@example.com");
        }

        [Fact]
        public async Task Login_ThrowsException_WhenCredentialsAreInvalid()
        {
            _mockUserRepository.Setup(repo => repo.All()).ReturnsAsync(new List<User>());

            Func<Task> act = async () => await _userService.Login("juan@example.com", "wrongpassword");

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Correo o contraseña incorrectos.");
        }

        [Fact]
        public async Task Delete_ThrowsException_WhenUserNotFound()
        {
            _mockUserRepository.Setup(repo => repo.Detail(It.IsAny<int>())).Returns((User)null!);

            Func<Task> act = async () => await _userService.Delete(99);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("El usuario con el ID especificado no existe.");
        }

        [Fact]
        public async Task Delete_RemovesUser_WhenUserExists()
        {
            var user = new User { UserId = 1, Name = "Juan" };
            _mockUserRepository.Setup(repo => repo.Detail(1)).Returns(user);

            var result = await _userService.Delete(1);

            result.Status.Should().Be(200);
            result.Message.Should().Be("Usuario eliminando exitosamente");

            _mockUserRepository.Verify(repo => repo.Delete(user), Times.Once);
        }
    }
}
