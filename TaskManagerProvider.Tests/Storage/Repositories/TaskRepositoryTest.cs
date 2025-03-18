using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Models;
using TaskManagerProvider.Storage.Repositories;
using Task = TaskManager.Models.Task;
using TTask = System.Threading.Tasks.Task;

namespace TaskManagerProvider.Tests.Storage.Repositories;

public class TaskRepositoryTest : IDisposable
{
    private Mock<IDefaultDataProvider<Task>> _defaultTaskDataProviderMock;
    private Mock<ILogger<Task>> _loggerMock;
    private TaskRepository _taskRepository;

    public TaskRepositoryTest()
    {
        var defaultTaskData = new List<Task>([
            new Task { Name = "Complete project", UserId = 1, State = (global::TaskManager.Models.TaskState)TaskState.New },
            new Task { Name = "Review code", UserId = 1, State = (global::TaskManager.Models.TaskState)TaskState.InProgress },
            new Task { Name = "Write documentation", UserId = 2, State = (global::TaskManager.Models.TaskState)TaskState.New }
        ]);

        _defaultTaskDataProviderMock = new Mock<IDefaultDataProvider<Task>>();
        _defaultTaskDataProviderMock.Setup(x => x.DefaultData).Returns(defaultTaskData.AsReadOnly());

        _loggerMock = new Mock<ILogger<Task>>();
        _taskRepository = new TaskRepository(_defaultTaskDataProviderMock.Object, _loggerMock.Object);
    }

    public void Dispose()
    {
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async TTask TaskRepository_GettingNotExistingTaskById()
    {
        const int testTaskId = 99;

        var ex = await Assert.ThrowsAnyAsync<KeyNotFoundException>(() =>
            _taskRepository.GetTaskById(testTaskId));

        Assert.True(ex.Message == $"Expected {nameof(Task)} record for key {testTaskId} not found.",
            "The exception message needs to be as expected.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async TTask TaskRepository_GettingExistingTaskById(int testTaskId)
    {
        var result = await _taskRepository.GetTaskById(testTaskId);

        Assert.True(result.Id == testTaskId, "Test and actual task's Id should be equal.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async TTask TaskRepository_GettingTasksByUserId(int testUserId)
    {
        var result = await _taskRepository.GetUserTasks(testUserId);

        Assert.True(result.Count() > 0, "User related tasks collection shouldn't be empty.");
    }

    [Fact]
    public async TTask TaskRepository_CreateTaskNotNull()
    {
        var testTask = new Task() { Name = "Test Task Name", State = (global::TaskManager.Models.TaskState)TaskState.InProgress, UserId = 2 } ;

        var createdTask = await _taskRepository.CreateTask(testTask);
        testTask.Id = createdTask.Id;

        Assert.True(createdTask == testTask, "All data of the recently added task should be the same as the source record except Id.");
    }

    [Fact]
    public async TTask TaskRepository_CreateAndUpdateTask()
    {
        var testTask = new Task() { Name = "Test Task Name", State = (global::TaskManager.Models.TaskState)TaskState.InProgress, UserId = 2 } ;

        var createdTask = await _taskRepository.CreateTask(testTask);
        testTask.Id = createdTask.Id;

        Assert.True(createdTask == testTask, "All data of the recently added task should be the same as the source record except Id.");

        var modifiedTask = createdTask with { Name = "Modified Test Task Name", State = (global::TaskManager.Models.TaskState)TaskState.Close, UserId = 1 };

        var updatedTask = await _taskRepository.UpdateTask(modifiedTask);

        Assert.True(updatedTask == modifiedTask, "All fields of the modified and actually updated record should be the same.");
    }

    [Fact]
    public async TTask TaskRepository_CreateUpdateNonExistingTask()
    {
        const int testTaskId = 99;

        var testTask = new Task() { Id = testTaskId, Name = "Test Task Name", State = (global::TaskManager.Models.TaskState)TaskState.InProgress, UserId = 2 } ;

        var updatedTask = await _taskRepository.UpdateTask(testTask);

        Assert.True(updatedTask == null, "The resulted null value should indicate that initial task record wasn't found.");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals($"Unable to update task with the specified Id: {testTaskId}. Record doesn't exist.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async TTask TaskRepository_DeleteTaskNotNull()
    {
        const int testTaskId = 2;

        var isRemoved = await _taskRepository.DeleteTask(testTaskId);

        Assert.True(isRemoved, "First time the task with the specified Id should be completely removed.");

        isRemoved = await _taskRepository.DeleteTask(testTaskId);

        Assert.False(isRemoved, "Second time the task with the specified Id should not be found to remove.");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals($"Unable to delete task with the specified Id: {testTaskId}. Record doesn't exist.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Theory]
    [InlineData(new [] {1, 2})]
    [InlineData(new [] {3})]
    public async TTask TaskRepository_DeleteTasks(int[] testTaskIds)
    {
        List<Task> tasksToDelete = testTaskIds.Select(taskId => new Task() { Id = taskId, Name = $"Task {taskId}" }).ToList();

        var results = await _taskRepository.DeleteTasks(tasksToDelete);

        Assert.True(results.All(b => b), "All listed tasks should be successfully removed.");

        foreach (var taskId in testTaskIds)
        {
            var ex = await Assert.ThrowsAnyAsync<KeyNotFoundException>(() =>
                _taskRepository.GetTaskById(taskId));
        }
    }
}