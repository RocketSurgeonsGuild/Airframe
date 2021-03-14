using System;
using Android.Gms.Location;

namespace Rocket.Surgery.Airframe.Droid
{
    public interface ILocationCallbackService
    {
        /// <summary>
        /// Gets an observable sequence of <see cref="LocationRequest"/>.
        /// </summary>
        IObservable<LocationResult> LocationChanged { get; }

        /// <summary>
        /// Gets an observable sequence of <see cref="LocationAvailability"/>.
        /// </summary>
        IObservable<LocationAvailability> LocationAvailabilityChanged { get; }
    }
}