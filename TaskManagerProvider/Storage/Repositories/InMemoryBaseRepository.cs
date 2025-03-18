using System.Collections.Concurrent;
using TaskManagerProvider.Domain.Repositories;

namespace TaskManagerProvider.Storage.Repositories
{
    public class InMemoryBaseRepository<TModel> : IInMemoryBaseRepository<TModel>
    {
        private int _lastId = 0;
        
        protected ILogger<TModel> Logger { get; private set; }
        protected IDictionary<int, TModel> Collection { get; private set; }

        public InMemoryBaseRepository(ILogger<TModel> logger)
        {
            System.ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            Logger = logger;
            Collection = new ConcurrentDictionary<int, TModel>();
        }

        public int GetNextId() => Interlocked.Increment(ref _lastId);

        public Task<IEnumerable<TModel>> RetrieveAll()
        {
            Logger.LogTrace("Getting all {0}", typeof(TModel).Name);

            return Task.FromResult(Collection.ToArray().Select(s => s.Value));
        }
    }
}