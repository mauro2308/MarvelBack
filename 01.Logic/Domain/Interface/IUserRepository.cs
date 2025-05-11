using Domain.Entity;

namespace Domain.Interface;

public interface IUserRepository
{
    Task<IEnumerable<User>> All();
    Task<User> Create(User user);
    User? Detail(int userId);
    Task Update(User user);
    Task Delete(User user);
}