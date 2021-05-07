using ReactiveUI;

namespace Airframe.Tests.Shiny.Settings
{
    internal class TestObject : ReactiveObject
    {
        
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}