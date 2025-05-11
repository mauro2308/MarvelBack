using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using BusinessLogic.Ports;
using Domain.CustomEntities;
using Domain.Entity;
using Domain.Exceptions;
using Domain.Interface;
using Infrastructure.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Adapter.Usuario;

public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
{
    public async Task<IEnumerable<UserDto>> All()
    {
        var user = await userRepository.All();
        var users = user.Select(x => new UserDto
        {
            UserId = x.UserId,
            Name = x.Name,
            Identification = x.Identification,
            Email = x.Email
        }).ToList();

        return users;
    }

    public async Task<Response> Create(UserDto row)
    {
        var existingUser = await userRepository.All();
        if (existingUser.Any(u => u.Name == row.Name))
            throw new BusinessException(HttpStatusCode.BadRequest, "Error usuario",
                "Ya existe un usuario con el mismo nombre.");
        var user = new User
        {
            Name = row.Name,
            Identification = row.Identification,
            Password = row.Password,
            Email = row.Email
        };
        await userRepository.Create(user);
        return new Response { Status = 200, Message = "Usuario creado exitosamente" };
    }

    public async Task<Response> Update(UserDto row)
    {
        var result = userRepository.Detail(row.UserId);
        if (result == null)
            throw new BusinessException(HttpStatusCode.NotFound, "Error usuario",
                "El usuario con el ID especificado no existe.");

        result.Name = row.Name;
        result.Identification = row.Identification;
        result.Password = row.Password;
        result.Email = row.Email;

        await userRepository.Update(result);

        return new Response { Status = StatusCodes.Status200OK, Message = "Usuario actualizado exitosamente" };
    }

    public async Task<Response> Delete(int? userId)
    {
        var result = userRepository.Detail((int)userId!);
        if (result == null)
            throw new BusinessException(HttpStatusCode.NotFound, "Error usuario",
                "El usuario con el ID especificado no existe.");

        await userRepository.Delete(result);
        return new Response { Status = StatusCodes.Status200OK, Message = "Usuario eliminando exitosamente" };
    }

    public async Task<string> Login(string email, string password)
    {
        var user = (await userRepository.All()).FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user == null)
            throw new BusinessException(HttpStatusCode.Unauthorized, "Credenciales Inválidas", "Correo o contraseña incorrectos.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credential
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}