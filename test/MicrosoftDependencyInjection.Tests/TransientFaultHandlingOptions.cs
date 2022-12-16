using System;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests
{
    public sealed class TransientFaultHandlingOptions
    {
        public bool Enabled { get; set; }
        public TimeSpan AutoRetryDelay { get; set; }
    }
}