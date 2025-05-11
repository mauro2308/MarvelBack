using Domain.CustomEntities;
using Infrastructure.Dto;

namespace BusinessLogic.Ports;

public interface IUserService
{
    Task<IEnumerable<UserDto>> All();
    Task<Response> Create(UserDto row);
    Task<Response> Update(UserDto row);
    Task<Response> Delete(int? userId);
    Task<string> Login(string email, string password);
}