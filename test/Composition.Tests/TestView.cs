using ReactiveUI;

namespace Composition.Tests
{
    public class TestView : IViewFor<TestViewModel>
    {
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel) value;
        }

        public TestViewModel ViewModel { get; set; }
    }
}