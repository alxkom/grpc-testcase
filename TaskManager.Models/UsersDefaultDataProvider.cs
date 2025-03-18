using TaskManager.Models;

public class UsersDefaultDataProvider : IDefaultDataProvider<User>
{
    protected readonly List<User> _users = new List<User>();

    public UsersDefaultDataProvider()
    {
        this._users.Add(new User { Name = "John Doe" });
        this._users.Add(new User { Name = "Jane Smith" });
    }

    public IReadOnlyCollection<User> DefaultData
    {
        get
        {
            return this._users.AsReadOnly();
        }
    }
}