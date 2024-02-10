using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveMarbles.Extensions;
using Rocket.Surgery.Airframe.Navigation;

namespace Rocket.Surgery.Airframe.ViewModels;

/// <summary>
/// Base view model for navigation.
/// </summary>
public abstract class NavigableViewModelBase : ViewModelBase, INavigated, IDestructible, IInitialize
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigableViewModelBase"/> class.
    /// </summary>
    protected NavigableViewModelBase()
    {
        _initialize = new AsyncSubject<IArguments>().DisposeWith(Garbage);
        _navigatedTo = new Subject<IArguments>().DisposeWith(Garbage);
        _navigatedFrom = new Subject<IArguments>().DisposeWith(Garbage);
    }

    /// <inheritdoc/>
    void IInitialize.OnInitialize(IArguments arguments)
    {
        _initialize.OnNext(arguments);
        _initialize.OnCompleted();
    }

    /// <inheritdoc/>
    void INavigated.OnNavigatedTo(IArguments arguments) => _navigatedTo.OnNext(arguments);

    /// <inheritdoc/>
    void INavigated.OnNavigatedFrom(IArguments arguments) => _navigatedFrom.OnNext(arguments);

    /// <summary>
    /// Gets the initial to <see cref="IArguments"/>.
    /// </summary>
    /// <returns>The arguments.</returns>
    protected IObservable<IArguments> Initialize => _initialize.AsObservable().Publish().RefCount();

    /// <summary>
    /// Gets the navigated to <see cref="IArguments"/>.
    /// </summary>
    /// <returns>The arguments.</returns>
    protected IObservable<IArguments> NavigatedTo => _navigatedTo.AsObservable().Publish().RefCount();

    /// <summary>
    /// Gets the navigated from <see cref="IArguments"/>.
    /// </summary>
    /// <returns>The arguments.</returns>
    protected IObservable<IArguments> NavigatedFrom => _navigatedFrom.AsObservable().Publish().RefCount();

    /// <inheritdoc/>
    void IDestructible.Destroy()
    {
        Destroy();
        Dispose();
    }

    /// <summary>
    /// Template method to destroy the view model.
    /// </summary>
    protected virtual void Destroy()
    {
    }

#pragma warning disable CA2213
    private readonly AsyncSubject<IArguments> _initialize;
    private readonly Subject<IArguments> _navigatedTo;
    private readonly Subject<IArguments> _navigatedFrom;
#pragma warning restore CA2213
}