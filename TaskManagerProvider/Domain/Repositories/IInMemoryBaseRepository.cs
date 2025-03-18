namespace TaskManagerProvider.Domain.Repositories
{
    public interface IInMemoryBaseRepository<T>
    {
        Task<IEnumerable<T>> RetrieveAll();
    }
}