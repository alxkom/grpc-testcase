using TaskManager.Models;
using Task = TaskManager.Models.Task;

namespace TaskManager.Services;

public interface IDataService
{
    // User operations
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(User user);
    Task<int> DeleteUserAsync(int userId);

    // Task operations
    Task<List<Task>> GetTasksAsync();
    Task<List<Task>> GetUserTasksAsync(int userId);
    Task<Task?> GetTaskByIdAsync(int taskId);
    Task<Task> CreateTaskAsync(Task task);
    Task<Task?> UpdateTaskAsync(Task task);
    Task<int> DeleteTaskAsync(int taskId);
}