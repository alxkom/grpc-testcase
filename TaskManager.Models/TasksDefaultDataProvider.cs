using TaskManager.Models;
using Task = TaskManager.Models.Task;

public class TasksDefaultDataProvider : IDefaultDataProvider<Task>
{
    protected readonly List<Task> _tasks = new List<Task>();

    public TasksDefaultDataProvider()
    {
        this._tasks.Add(new Task { Name = "Complete project", UserId = 1, State = TaskState.New });
        this._tasks.Add(new Task { Name = "Review code", UserId = 1, State = TaskState.InProgress });
        this._tasks.Add(new Task { Name = "Write documentation", UserId = 2, State = TaskState.New });
    }

    public IReadOnlyCollection<Task> DefaultData
    {
        get
        {
            return this._tasks.AsReadOnly();
        }
    }
}