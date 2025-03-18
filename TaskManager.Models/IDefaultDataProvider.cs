public interface IDefaultDataProvider<T> where T : class
{
    IReadOnlyCollection<T> DefaultData { get; }
}