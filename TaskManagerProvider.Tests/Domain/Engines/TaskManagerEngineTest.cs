using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Models;
using TaskManagerProvider.Domain.Engines;
using TaskManagerProvider.Domain.Repositories;
using Task = TaskManager.Models.Task;
using TTask = System.Threading.Tasks.Task;

namespace TaskManagerProvider.Tests.Domain.Engines;

public class TaskManagerEngineTest : IDisposable
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ITaskRepository> _taskRepositoryMock;
    private Mock<ILogger<TaskManagerEngine>> _loggerMock;
    private TaskManagerEngine _taskManagerEngine;

    public TaskManagerEngineTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _taskRepositoryMock = new Mock<ITaskRepository>();

        _loggerMock = new Mock<ILogger<TaskManagerEngine>>();
        _taskManagerEngine = new TaskManagerEngine(_userRepositoryMock.Object, _taskRepositoryMock.Object, _loggerMock.Object);
    }

    public void Dispose()
    {
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async TTask TaskManagerEngine_DeleteUserAndAllRelatedTasks()
    {
        const int testUserId = 1;

        var relatedTasks = new List<Task>([
            new Task { Id = 1, Name = "Complete project", UserId = 1, State = (global::TaskManager.Models.TaskState)TaskState.New },
            new Task { Id = 2, Name = "Review code", UserId = 1, State = (global::TaskManager.Models.TaskState)TaskState.InProgress },
            new Task { Id = 3, Name = "Write documentation", UserId = 2, State = (global::TaskManager.Models.TaskState)TaskState.New }
        ]).Where(task => task.UserId == testUserId).ToList();

        var relatedTasksCount = relatedTasks.Count(t => t.UserId == testUserId);

        _taskRepositoryMock.Setup(x => x.GetUserTasks(It.Is<int>(t => t == testUserId))).Returns(TTask.FromResult(relatedTasks.AsEnumerable()));
        _taskRepositoryMock.Setup(x => x.DeleteTasks(It.IsAny<IEnumerable<Task>>())).Returns(TTask.FromResult(relatedTasks.Select(t => true).ToArray()));
        _userRepositoryMock.Setup(x => x.DeleteUser(It.Is<int>(id => id == testUserId))).Returns(TTask.FromResult(true));

        var result = await _taskManagerEngine.DeleteUser(testUserId);

        Assert.True(result, "The user with the specified Id and all related tasks should be successfully removed.");
        _taskRepositoryMock.Verify(x => x.GetUserTasks(It.Is<int>(t => t == testUserId)), Times.Once());
        _taskRepositoryMock.Verify(x => x.DeleteTasks(It.IsAny<IEnumerable<Task>>()), Times.Once());
        _userRepositoryMock.Verify(x => x.DeleteUser(It.Is<int>(id => id == testUserId)), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals($"{relatedTasksCount} related tasks were deleted for user with the specifed user Id: {testUserId}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async TTask TaskManagerEngine_DeleteUserButNotAllRelatedTasks()
    {
        const int testUserId = 1;

        var relatedTasks = new List<Task>([
            new Task { Id = 1, Name = "Complete project", UserId = 1, State = (global::TaskManager.Models.TaskState)TaskState.New },
            new Task { Id = 2, Name = "Review code", UserId = 1, State = (global::TaskManager.Models.TaskState)TaskState.InProgress },
            new Task { Id = 3, Name = "Write documentation", UserId = 2, State = (global::TaskManager.Models.TaskState)TaskState.New }
        ]).Where(task => task.UserId == testUserId).ToList();

        var relatedTasksCount = relatedTasks.Count(t => t.UserId == testUserId);

        _taskRepositoryMock.Setup(x => x.GetUserTasks(It.Is<int>(t => t == testUserId))).Returns(TTask.FromResult(relatedTasks.AsEnumerable()));
        _taskRepositoryMock.Setup(x => x.DeleteTasks(It.IsAny<IEnumerable<Task>>())).Returns(TTask.FromResult(relatedTasks.Select(t => t.Id % 2 == 0).ToArray()));
        _userRepositoryMock.Setup(x => x.DeleteUser(It.Is<int>(id => id == testUserId))).Returns(TTask.FromResult(true));

        var result = await _taskManagerEngine.DeleteUser(testUserId);

        Assert.False(result, "The user with the specified Id and all related tasks haven't been successfully removed.");
        _taskRepositoryMock.Verify(x => x.GetUserTasks(It.Is<int>(t => t == testUserId)), Times.Once());
        _taskRepositoryMock.Verify(x => x.DeleteTasks(It.IsAny<IEnumerable<Task>>()), Times.Once());
        _userRepositoryMock.Verify(x => x.DeleteUser(It.Is<int>(id => id == testUserId)), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals($"Not all related tasks were deleted for user with the specifed user Id: {testUserId}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }
}