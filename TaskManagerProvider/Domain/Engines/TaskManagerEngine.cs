using TaskManager.Models;
using TaskManagerProvider.Domain.Repositories;
using Task = TaskManager.Models.Task;

namespace TaskManagerProvider.Domain.Engines
{
    public class TaskManagerEngine : ITaskManagerEngine
    {
        protected IUserRepository UserRepository { get; set; }
        protected ITaskRepository TaskRepository { get; set; }
        protected ILogger<TaskManagerEngine> Logger { get; set; }

        public TaskManagerEngine(IUserRepository userRepository, ITaskRepository taskRepository, ILogger<TaskManagerEngine> logger)
        {
            UserRepository = userRepository.ThrowIfNull();
            TaskRepository = taskRepository.ThrowIfNull();
            Logger = logger.ThrowIfNull();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await UserRepository.RetrieveAll();

            return users;
        }

        public async Task<User> GetUserById(int userId)
        {
            var user = await UserRepository.GetUserById(userId);
            return user;
        }

        public async Task<User> CreateUser(User user)
        {
            user.ThrowIfNull();

            var newUser = await UserRepository.CreateUser(user);
            return newUser;
        }

        public async Task<User?> UpdateUser(User user)
        {
            user.ThrowIfNull();

            var updatedUser = await UserRepository.UpdateUser(user);
            return updatedUser;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var relatedTasks = await TaskRepository.GetUserTasks(userId);

            var relatedTasksResult = await TaskRepository.DeleteTasks(relatedTasks);
            bool allTasksRemoved = relatedTasksResult.All(isRemoved => isRemoved);

            if (!allTasksRemoved)
            {
                Logger.LogWarning("Not all related tasks were deleted for user with the specifed user Id: {0}", userId);
            }
            else
            {
                Logger.LogInformation("{0} related tasks were deleted for user with the specifed user Id: {1}", relatedTasksResult.Count(), userId);
            }

            bool result = await UserRepository.DeleteUser(userId);
            return result && allTasksRemoved;
        }
 
        public async Task<IEnumerable<Task>> GetTasks()
        {
            var tasks = await TaskRepository.RetrieveAll();
            foreach (var task in tasks)
            {
                var user = await UserRepository.GetUserById(task.UserId);
                task.User = user;
            }

            return tasks;
        }

        public async Task<IEnumerable<Task>> GetUserTasks(int userId)
        {
            var user = await UserRepository.GetUserById(userId);

            var filteredTasks = await TaskRepository.GetUserTasks(userId);

            foreach (var task in filteredTasks)
            {
                task.User = user;
            }

            return filteredTasks;
        }

        public async Task<Task> GetTaskById(int taskId)
        {
            var task = await TaskRepository.GetTaskById(taskId);

            var user = await UserRepository.GetUserById(task.UserId);

            task.User = user;

            return task;
        }

        public async Task<Task> CreateTask(Task task)
        {
            task.ThrowIfNull();

            var user = await UserRepository.GetUserById(task.UserId);

            task.User = user;

            var newTask = await TaskRepository.CreateTask(task);

            return newTask;
        }

        public async Task<Task?> UpdateTask(Task task)
        {
            task.ThrowIfNull();

            var user = await UserRepository.GetUserById(task.UserId);

            task.User = user;

            var updatedTask = await TaskRepository.UpdateTask(task);

            return updatedTask;
        }

        public async Task<bool> DeleteTask(int taskId)
        {
            bool result = await TaskRepository.DeleteTask(taskId);
            return result;
        }
    }
}