using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using DynamicData;
using JetBrains.Annotations;

namespace Data
{
    /// <summary>
    /// Respresents a base <see cref="IDataService{T}"/> implementation.
    /// </summary>
    /// <typeparam name="T">The data transfer object type.</typeparam>
    public abstract class DataServiceBase<T> : IDataService<T>
        where T : Dto
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase{T}"/> class.
        /// </summary>
        /// <param name="client">The abstracted client.</param>
        protected DataServiceBase([NotNull] IClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public virtual IObservable<IChangeSet<T, Guid>> ChangeSet => SourceCache.Connect().RefCount();

        /// <summary>
        /// Gets the source cache.
        /// </summary>
        protected SourceCache<T, Guid> SourceCache { get; } = new SourceCache<T, Guid>(x => x.Id);

        /// <inheritdoc />
        public virtual IObservable<Unit> Create(T dto) =>
            Observable.Create<Unit>(_ =>
            {
                _client.Post(dto).ToObservable().Subscribe();

                using var x = _semaphore.WaitAsync().ToObservable().Subscribe();
                SourceCache.AddOrUpdate(dto);

                return x;
            });

        /// <inheritdoc />
        public virtual IObservable<T> Read() =>
            _client
                .GetAll<T>()
                .ToObservable()
                .SelectMany(x => x)
                .Where(x => x != null)
                .Do(_ => SourceCache.AddOrUpdate(_));

        /// <inheritdoc />
        public virtual IObservable<T> Read(Guid id) =>
            _client
                .Get<T>(id)
                .ToObservable()
                .Where(x => x != null)
                .Do(_ => SourceCache.AddOrUpdate(_));

        /// <inheritdoc />
        public virtual IObservable<Unit> Update(T dto) =>
            Observable.Create<Unit>(_ =>
            {
                var clientSubscription = _client.Post(dto).ToObservable().Subscribe();

                using var x = _semaphore.WaitAsync().ToObservable().Subscribe();
                SourceCache.AddOrUpdate(dto);

                return Disposable.Create(() =>
                {
                    x.Dispose();
                    clientSubscription.Dispose();
                });
            });

        /// <inheritdoc />
        public virtual IObservable<Unit> Delete(T dto) =>
            Observable.Create<Unit>(_ =>
            {
                _client.Delete(dto).ToObservable().Subscribe();

                using var x = _semaphore.WaitAsync().ToObservable().Subscribe();
                SourceCache.Remove(dto);

                return x;
            });

        /// <inheritdoc/>
        public IObservable<Unit> Delete(Guid id)
        {
            var dto = SourceCache.Lookup(id);

            return Delete(dto.Value);
        }

        /// <inheritdoc/>
        IObservable<Unit> IDataService.Create(object dto) => Create((T)dto);

        /// <inheritdoc/>
        IObservable<object> IDataService.Read() => Read();

        /// <inheritdoc/>
        IObservable<object> IDataService.Read(Guid id) => Read(id);

        /// <inheritdoc/>
        IObservable<Unit> IDataService.Update(object dto) => Update((T)dto);

        /// <inheritdoc/>
        IObservable<Unit> IDataService.Delete(object dto) => Delete((T)dto);
    }
}