using AutoMapper;
using Grpc.Core;
using TaskManager.Models;
using TaskManagerProvider.Domain.Engines;
using Task = TaskManager.Models.Task;

namespace TaskManagerProvider.Services
{
    public class TaskManagerService : TaskManager.TaskManagerBase
    {
        protected ITaskManagerEngine TaskManagerEngine { get; set; }
        protected IMapper Mapper { get; set; }
        protected ILogger<TaskManagerService> Logger { get; set; }

        public TaskManagerService(ITaskManagerEngine taskManagerEngine, IMapper mapper, ILogger<TaskManagerService> logger)
        {
            TaskManagerEngine = taskManagerEngine.ThrowIfNull();
            Mapper = mapper.ThrowIfNull();
            Logger = logger.ThrowIfNull();
        }

        public override async Task<UsersResponse> GetUsers(Empty request, ServerCallContext context)
        {
            Logger.LogInformation("GetUser is called from the client.");
            
            UsersResponse usersResponse = new UsersResponse();
            var users = await TaskManagerEngine.GetUsers();

            foreach (var user in users) 
            {
                usersResponse.Users.Add(Mapper.Map<UserItem>(user));
            }

            return usersResponse;
        }

        public override async Task<UserItem> GetUserById(UserKey request, ServerCallContext context)
        {
            request.ThrowIfNull();

            var user = await TaskManagerEngine.GetUserById(request.UserId);
            return Mapper.Map<UserItem>(user);
        }

        public override async Task<UserItem> CreateUser(UserItem request, ServerCallContext context)
        {
            request.ThrowIfNull();

            var user = await TaskManagerEngine.CreateUser(Mapper.Map<User>(request));
            return Mapper.Map<UserItem>(user);
        }

        public override async Task<UserItem> UpdateUser(UserItem request, ServerCallContext context)
        {
            request.ThrowIfNull();

            var user = await TaskManagerEngine.UpdateUser(Mapper.Map<User>(request));
            return Mapper.Map<UserItem>(user);
        }

        public override async Task<Empty> DeleteUser(UserKey request, ServerCallContext context)
        {
            request.ThrowIfNull();

            await TaskManagerEngine.DeleteUser(request.UserId);
            return new Empty();
        }

        public override async Task<TasksResponse> GetTasks(Empty request, ServerCallContext context)
        {
            TasksResponse tasksResponse = new TasksResponse();
            var tasks = await TaskManagerEngine.GetTasks();

            foreach (var task in tasks) 
            {
                tasksResponse.Tasks.Add(Mapper.Map<TaskManagerProvider.TaskItem>(task));
            }

            return tasksResponse;
        }

        public override async Task<TasksResponse> GetUserTasks(UserKey request, ServerCallContext context)
        {
            request.ThrowIfNull();

            TasksResponse tasksResponse = new TasksResponse();
            var tasks = await TaskManagerEngine.GetUserTasks(request.UserId);

            foreach (var task in tasks)
            {
                tasksResponse.Tasks.Add(Mapper.Map<TaskManagerProvider.TaskItem>(task));
            }

            return tasksResponse;
        }

        public override async Task<TaskItem> GetTaskById(TaskKey request, ServerCallContext context)
        {
            request.ThrowIfNull();

            var task = await TaskManagerEngine.GetTaskById(request.TaskId);
            return Mapper.Map<TaskItem>(task);
        }

        public override async Task<TaskItem> CreateTask(TaskItem request, ServerCallContext context)
        {
            request.ThrowIfNull();

            var task = await TaskManagerEngine.CreateTask(Mapper.Map<Task>(request));
            return Mapper.Map<TaskItem>(task);
        }

        public override async Task<TaskItem> UpdateTask(TaskItem request, ServerCallContext context)
        {
            request.ThrowIfNull();

            var task = await TaskManagerEngine.UpdateTask(Mapper.Map<Task>(request));
            return Mapper.Map<TaskItem>(task);
        }

        public override async Task<Empty> DeleteTask(TaskKey request, ServerCallContext context)
        {
            request.ThrowIfNull();

            await TaskManagerEngine.DeleteTask(request.TaskId);
            return new Empty();
        }
    }
}
