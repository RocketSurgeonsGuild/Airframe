//HintName: 

using System;
using ReactiveUI;

namespace Sample
{
    public class DirectAssignmentBlockExample : ReactiveObject
    {
        public string Name
        {
            get { return _name; }
            set {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }

        private string _name;
    }
}