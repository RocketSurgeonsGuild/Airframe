using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using DynamicData;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Represents a base <see cref="IDataService{T}"/> implementation.
    /// </summary>
    /// <typeparam name="T">The data transfer object type.</typeparam>
    public abstract class DataServiceBase<T> : IDataService<T>, IDisposable
        where T : IDto
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase{T}"/> class.
        /// </summary>
        /// <param name="client">The abstracted client.</param>
        protected DataServiceBase([NotNull] IClient client) => _client = client;

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
                var clientSubscription = Observable.FromAsync(() => _client.Post(dto)).Subscribe();

                using var x = Observable.FromAsync(() => _semaphore.WaitAsync()).Subscribe(_);
                SourceCache.AddOrUpdate(dto);

                return Disposable.Create(() => clientSubscription.Dispose());
            });

        /// <inheritdoc />
        public virtual IObservable<T> Read() => Observable
           .FromAsync(() => _client.GetAll<T>())
           .SelectMany(x => x)
           .Where(x => x != null)
           .Do(_ => SourceCache.AddOrUpdate(_));

        /// <inheritdoc />
        public virtual IObservable<T> Read(Guid id) => Observable
           .FromAsync(() => _client.Get<T>(id))
           .Where(x => x != null)
           .Do(_ => SourceCache.AddOrUpdate(_));

        /// <inheritdoc />
        public virtual IObservable<Unit> Update(T dto) =>
            Observable.Create<Unit>(_ =>
            {
                var clientSubscription = Observable.FromAsync(() => _client.Post(dto)).Subscribe();

                using var x = Observable.FromAsync(() => _semaphore.WaitAsync()).Subscribe(_);
                SourceCache.AddOrUpdate(dto);

                return Disposable.Create(() => clientSubscription.Dispose());
            });

        /// <inheritdoc />
        public virtual IObservable<Unit> Delete(T dto) =>
            Observable.Create<Unit>(_ =>
            {
                var clientSubscription = Observable.FromAsync(() => _client.Delete(dto)).Subscribe();
                using var x = Observable.FromAsync(() => _semaphore.WaitAsync()).Subscribe(_);
                SourceCache.Remove(dto);

                return Disposable.Create(() => clientSubscription.Dispose());
            });

        /// <inheritdoc/>
        public IObservable<Unit> Delete(Guid id)
        {
            var dto = SourceCache.Lookup(id);

            return Delete(dto.Value);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        IObservable<Unit> IDataService.Create(object dto) => Create((T)dto);

        /// <inheritdoc/>
        IObservable<object> IDataService.Read() => Read().Select(x => x as object);

        /// <inheritdoc/>
        IObservable<object> IDataService.Read(Guid id) => Read(id).Select(x => x as object);

        /// <inheritdoc/>
        IObservable<Unit> IDataService.Update(object dto) => Update((T)dto);

        /// <inheritdoc/>
        IObservable<Unit> IDataService.Delete(object dto) => Delete((T)dto);

        /// <summary>
        /// Disposes of resources when finalizing.
        /// </summary>
        /// <param name="disposing">A value indicating whether or not this instance is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _semaphore.Dispose();
                _client.Dispose();
                SourceCache.Dispose();
            }
        }
    }
}