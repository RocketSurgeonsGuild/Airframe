using System;

namespace Rocket.Surgery.ReactiveUI
{
    [AttributeUsage(AttributeTargets.All)]
    internal class PreserveAttribute : Attribute
    {
        public bool AllMembers { get; set; }
    }
}