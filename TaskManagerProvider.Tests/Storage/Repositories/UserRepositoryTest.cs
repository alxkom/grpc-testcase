using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Models;
using TaskManagerProvider.Storage.Repositories;
using TTask = System.Threading.Tasks.Task;

namespace TaskManagerProvider.Tests.Storage.Repositories;

public class UserRepositoryTest : IDisposable
{
    private Mock<IDefaultDataProvider<User>> _defaultUserDataProviderMock;
    private Mock<ILogger<User>> _loggerMock;
    private UserRepository _userRepository;

    public UserRepositoryTest()
    {
        var defaultUserData = new List<User>([
            new User { Name = "John Doe" },
            new User { Name = "Jane Smith" }
        ]);

        _defaultUserDataProviderMock = new Mock<IDefaultDataProvider<User>>();
        _defaultUserDataProviderMock.Setup(x => x.DefaultData).Returns(defaultUserData.AsReadOnly());

        _loggerMock = new Mock<ILogger<User>>();
        _userRepository = new UserRepository(_defaultUserDataProviderMock.Object, _loggerMock.Object);
    }

    public void Dispose()
    {
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async TTask UserRepository_GettingNotExistingUserById()
    {
        const int testUserId = 99;

        var ex = await Assert.ThrowsAnyAsync<KeyNotFoundException>(() =>
            _userRepository.GetUserById(testUserId));

        Assert.True(ex.Message == $"Expected {nameof(User)} record for key {testUserId} not found.",
            "The exception message needs to be as expected.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async TTask UserRepository_GettingExistingUserById(int testUserId)
    {
        var result = await _userRepository.GetUserById(testUserId);

        Assert.True(result.Id == testUserId, "Test and actual user's Id should be equal.");
    }

    [Fact]
    public async TTask UserRepository_CreateUserNotNull()
    {
        var testUser = new User() { Name = "Test User Name" } ;

        var createdUser = await _userRepository.CreateUser(testUser);
        testUser.Id = createdUser.Id;

        Assert.True(createdUser == testUser, "All data of the recently added user should be the same as the source record except Id.");
    }

    [Fact]
    public async TTask UserRepository_CreateAndUpdateUser()
    {
        var testUser = new User() { Name = "Test User Name" } ;

        var createdUser = await _userRepository.CreateUser(testUser);
        testUser.Id = createdUser.Id;

        Assert.True(createdUser == testUser, "All data of the recently added user should be the same as the source record except Id.");

        var modifiedUser = createdUser with { Name = "Modified Test User Name" };

        var updatedUser = await _userRepository.UpdateUser(modifiedUser);

        Assert.True(updatedUser == modifiedUser, "All fields of the modified and actually updated record should be the same.");
    }

    [Fact]
    public async TTask UserRepository_CreateUpdateNonExistingUser()
    {
        const int testUserId = 99;

        var testUser = new User() { Id = testUserId,  Name = "Test User Name" } ;

        var updatedUser = await _userRepository.UpdateUser(testUser);

        Assert.True(updatedUser == null, "The resulted null value should indicate that initial user record wasn't found.");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals($"Unable to update user with the specified Id: {testUserId}. Record doesn't exist.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async TTask UserRepository_DeleteUserNotNull()
    {
        const int testUserId = 2;

        var isRemoved = await _userRepository.DeleteUser(testUserId);

        Assert.True(isRemoved, "First time the user with the specified Id should be completely removed.");

        isRemoved = await _userRepository.DeleteUser(testUserId);

        Assert.False(isRemoved, "Second time the user with the specified Id should not be found to remove.");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals($"Unable to delete user with the specified Id: {testUserId}. Record doesn't exist.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }
}