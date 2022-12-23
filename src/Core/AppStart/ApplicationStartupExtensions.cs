using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Future default interface implementations. Some call them mixins.
    /// </summary>
    public static class ApplicationStartupExtensions
    {
        /// <summary>
        /// Starts the application life cycle.
        /// </summary>
        /// <param name="startup">The application startup.</param>
        /// <returns>A completion notification.</returns>
        public static IObservable<Unit> Startup(this IApplicationStartup startup) => startup.Startup(1);
    }
}