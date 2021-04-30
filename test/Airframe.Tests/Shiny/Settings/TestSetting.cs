using ReactiveUI;

namespace Airframe.Tests.Shiny.Settings
{
    internal class TestSetting : ReactiveObject
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}