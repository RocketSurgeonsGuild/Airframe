using ReactiveUI;

namespace Composition.Tests;

public class TestView : IViewFor<TestViewModel>
{
    /// <inheritdoc/>
    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TestViewModel) value;
    }

    /// <inheritdoc/>
    public TestViewModel ViewModel { get; set; }
}