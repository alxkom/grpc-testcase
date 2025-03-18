using TaskManager.Models;
using TaskManagerProvider.Domain.Repositories;
using TTask = System.Threading.Tasks.Task;

namespace TaskManagerProvider.Storage.Repositories
{
    public class UserRepository : InMemoryBaseRepository<User>, IUserRepository
    {
        public UserRepository(IDefaultDataProvider<User> dataProvider, ILogger<User> logger) : base(logger)
        {
            foreach (var user in dataProvider.DefaultData)
            {
                user.Id = GetNextId();
                Collection.Add(user.Id, user);
            }
        }

        public Task<User> GetUserById(int userId)
        {
            Collection.TryGetValue(userId, out var user);
            return TTask.FromResult(user ?? 
                throw new KeyNotFoundException($"Expected {nameof(User)} record for key {userId} not found."));
        }

        public Task<User> CreateUser(User user)
        {
            user.ThrowIfNull();

            user.Id = GetNextId();
            if (Collection.TryAdd(user.Id, user))
            {
                return TTask.FromResult(user);
            }
            throw new InvalidOperationException("Failed to create a user");
        }

        public Task<User?> UpdateUser(User user)
        {
            user.ThrowIfNull();

            if (Collection.ContainsKey(user.Id))
            {
                Collection[user.Id] = user;
                return TTask.FromResult<User?>(user);
            }

            Logger.LogWarning("Unable to update user with the specified Id: {0}. Record doesn't exist.", user.Id);

            return TTask.FromResult<User?>(null);
        }

        public Task<bool> DeleteUser(int userId)
        {
            bool result = false;

            if (Collection.ContainsKey(userId))
            {
                result = Collection.Remove(userId);
            }

            if (!result)
            {
                Logger.LogWarning("Unable to delete user with the specified Id: {0}. Record doesn't exist.", userId);
            }

            return TTask.FromResult(result);
        }
    }
}