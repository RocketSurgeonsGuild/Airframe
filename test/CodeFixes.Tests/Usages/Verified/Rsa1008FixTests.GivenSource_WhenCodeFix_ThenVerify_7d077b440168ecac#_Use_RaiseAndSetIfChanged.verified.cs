//HintName: 

using System;
using ReactiveUI;

namespace Sample
{
    public class DirectAssignmentArrowExample : ReactiveObject
    {
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _name;
    }
}