using TaskManager.Models;
using Task = TaskManager.Models.Task;

namespace TaskManagerProvider.Domain.Engines
{
    public interface ITaskManagerEngine
    {
        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUserById(int userId);

        Task<User> CreateUser(User user);

        Task<User?> UpdateUser(User user);

        Task<bool> DeleteUser(int userId);

        Task<IEnumerable<Task>> GetTasks();

        Task<IEnumerable<Task>> GetUserTasks(int userId);

        Task<Task> GetTaskById(int taskId);

        Task<Task> CreateTask(Task task);

        Task<Task?> UpdateTask(Task task);

        Task<bool> DeleteTask(int taskId);
    }
}