using AutoMapper;
using TaskManager.Models;
using Task = TaskManager.Models.Task;

namespace TaskManager.Services;

public class DataService : IDataService
{
    private readonly IMapper _mapper;
    private readonly TaskManagerProvider.TaskManager.TaskManagerClient _client;

    public DataService(TaskManagerProvider.TaskManager.TaskManagerClient client, IMapper mapper)
    {
        System.ArgumentNullException.ThrowIfNull(client, nameof(client));
        System.ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        _client = client;
        _mapper = mapper;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        using (var call = _client.GetUsersAsync(new TaskManagerProvider.Empty()))
        {
            var response = await call.ConfigureAwait(false);

            return response.Users.Select(x => _mapper.Map<User>(x)).ToList();
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        using (var call = _client.GetUserByIdAsync(new TaskManagerProvider.UserKey() { UserId = userId }))
        {
            var userItem = await call.ConfigureAwait(false);

            return _mapper.Map<User>(userItem);
        }
    }

    public async Task<User> CreateUserAsync(User user)
    {
        System.ArgumentNullException.ThrowIfNull(user, nameof(user));

        using (var call = _client.CreateUserAsync(_mapper.Map<TaskManagerProvider.UserItem>(user)))
        {
            var userItem = await call.ConfigureAwait(false);

            return _mapper.Map<User>(userItem);
        }
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        System.ArgumentNullException.ThrowIfNull(user, nameof(user));

        using (var call = _client.UpdateUserAsync(_mapper.Map<TaskManagerProvider.UserItem>(user)))
        {
            var userItem = await call.ConfigureAwait(false);

            return userItem != null ? _mapper.Map<User>(userItem) : null;
        }
    }

    public async Task<int> DeleteUserAsync(int userId)
    {
        using (var call = _client.DeleteUserAsync(new TaskManagerProvider.UserKey() { UserId = userId }))
        {
            await call.ConfigureAwait(false);

            return userId;
        }
    }

    public async Task<List<Task>> GetTasksAsync()
    {
        using (var call = _client.GetTasksAsync(new TaskManagerProvider.Empty()))
        {
            var response = await call.ConfigureAwait(false);

            return response.Tasks.Select(x => _mapper.Map<Task>(x)).ToList();
        }
    }

    public async Task<List<Task>> GetUserTasksAsync(int userId)
    {
        using (var call = _client.GetUserTasksAsync(new TaskManagerProvider.UserKey() { UserId = userId }))
        {
            var response = await call.ConfigureAwait(false);

            return response.Tasks.Select(x => _mapper.Map<Task>(x)).ToList();
        }
    }

    public async Task<Task?> GetTaskByIdAsync(int taskId)
    {
        using (var call = _client.GetTaskByIdAsync(new TaskManagerProvider.TaskKey() { TaskId = taskId }))
        {
            var taskItem = await call.ConfigureAwait(false);

            return _mapper.Map<Task>(taskItem);
        }
    }

    public async Task<Task> CreateTaskAsync(Task task)
    {
        System.ArgumentNullException.ThrowIfNull(task, nameof(task));

        using (var call = _client.CreateTaskAsync(_mapper.Map<TaskManagerProvider.TaskItem>(task)))
        {
            var taskItem = await call.ConfigureAwait(false);

            return _mapper.Map<Task>(taskItem);
        }
    }

    public async Task<Task?> UpdateTaskAsync(Task task)
    {
        System.ArgumentNullException.ThrowIfNull(task, nameof(task));

        using (var call = _client.UpdateTaskAsync(_mapper.Map<TaskManagerProvider.TaskItem>(task)))
        {
            var taskItem = await call.ConfigureAwait(false);

            return _mapper.Map<Task>(taskItem);
        }
    }

    public async Task<int> DeleteTaskAsync(int taskId)
    {
        using (var call = _client.DeleteTaskAsync(new TaskManagerProvider.TaskKey() { TaskId = taskId }))
        {
            await call.ConfigureAwait(false);

            return taskId;
        }
    }
}