using TaskManager.Models;

namespace TaskManagerProvider.Domain.Repositories
{
    public interface IUserRepository : IInMemoryBaseRepository<User>
    {
        Task<User> GetUserById(int userId);

        Task<User> CreateUser(User user);

        Task<User?> UpdateUser(User user);

        Task<bool> DeleteUser(int userId);
    }
}