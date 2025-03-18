using Task = TaskManager.Models.Task;

namespace TaskManagerProvider.Domain.Repositories
{
    public interface ITaskRepository : IInMemoryBaseRepository<Task>
    {
        Task<Task> GetTaskById(int taskId);

        Task<IEnumerable<Task>> GetUserTasks(int userId);

        Task<Task> CreateTask(Task task);

        Task<Task?> UpdateTask(Task task);

        Task<bool> DeleteTask(int taskId);

        Task<bool[]> DeleteTasks(IEnumerable<Task> tasks);
    }
}