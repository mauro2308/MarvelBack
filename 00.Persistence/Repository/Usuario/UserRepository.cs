using Domain.Entity;
using Domain.Interface;
using Infrastructure.Interfaces;

namespace Repository.Usuario;

public class UserRepository(IUnitOfWork unitOfWork) : IUserRepository
{
    public Task<IEnumerable<User>> All()
    {
        var userRepository = unitOfWork.Repository<User>()!;
        return userRepository.GetAll();
    }

    public async Task<User> Create(User user)
    {
        var userRepository = unitOfWork.Repository<User>();
        userRepository?.Insert(user);
        await unitOfWork.Complete();
        return user;
    }

    public User? Detail(int userId)
    {
        var userRepository = unitOfWork.Repository<User>();
        return userRepository?.GetByID(userId);
    }

    public async Task Update(User user)
    {
        var userRepository = unitOfWork.Repository<User>();
        userRepository?.Update(user);
        await unitOfWork.Complete();
    }

    public async Task Delete(User user)
    {
        var userRepository = unitOfWork.Repository<User>();
        userRepository?.Delete(user);
        await unitOfWork.Complete();
    }
}