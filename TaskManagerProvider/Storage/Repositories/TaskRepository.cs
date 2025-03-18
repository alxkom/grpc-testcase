using TaskManagerProvider.Domain.Repositories;
using Task = TaskManager.Models.Task;

namespace TaskManagerProvider.Storage.Repositories
{
    public class TaskRepository : InMemoryBaseRepository<Task>, ITaskRepository
    {
        public TaskRepository(IDefaultDataProvider<Task> dataProvider, ILogger<Task> logger) : base(logger)
        {
            foreach (var task in dataProvider.DefaultData)
            {
                task.Id = GetNextId();
                Collection.Add(task.Id, task);
            }
        }

        public Task<Task> GetTaskById(int taskId)
        {
            Collection.TryGetValue(taskId, out var task);
            return System.Threading.Tasks.Task.FromResult(task ?? 
                throw new KeyNotFoundException($"Expected {nameof(Task)} record for key {taskId} not found."));
        }

        public Task<IEnumerable<Task>> GetUserTasks(int userId)
        {
            var filteredTasks = Collection.Values
                .Where(t => t.UserId == userId)
                .ToList();

            return System.Threading.Tasks.Task.FromResult(filteredTasks.AsEnumerable());
        }

        public Task<Task> CreateTask(Task task)
        {
            task.ThrowIfNull();

            task.Id = GetNextId();
            if (Collection.TryAdd(task.Id, task))
            {
                return System.Threading.Tasks.Task.FromResult(task);
            }
            throw new InvalidOperationException("Failed to create a task");
        }

        public Task<Task?> UpdateTask(Task task)
        {
            task.ThrowIfNull();

            if (Collection.ContainsKey(task.Id))
            {
                Collection[task.Id] = task;
                return System.Threading.Tasks.Task.FromResult<Task?>(task);
            }

            Logger.LogWarning("Unable to update user with the specified Id: {0}. Record doesn't exist.", task.Id);

            return System.Threading.Tasks.Task.FromResult<Task?>(null);
        }

        public Task<bool> DeleteTask(int taskId)
        {
            bool result = false;

            if (Collection.ContainsKey(taskId))
            {
                result = Collection.Remove(taskId);
            }

            if (!result)
            {
                Logger.LogWarning("Unable to delete user with the specified Id: {0}. Record doesn't exist.", taskId);
            }

            return System.Threading.Tasks.Task.FromResult(result);
        }

        public Task<bool[]> DeleteTasks(IEnumerable<Task> tasks)
        {
            tasks.ThrowIfNull();

            List<System.Threading.Tasks.Task<bool>> tasksToComplete =
                new List<System.Threading.Tasks.Task<bool>>(tasks.Count());

            foreach (var task in tasks)
            {
                tasksToComplete.Add(DeleteTask(task.Id));
            }

            return System.Threading.Tasks.Task.WhenAll(tasksToComplete);
        }
    }
}