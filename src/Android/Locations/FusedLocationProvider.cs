using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Gms.Location;
using Android.Locations;
using ILocationListener = Android.Gms.Location.ILocationListener;

namespace Rocket.Surgery.Airframe.Droid
{
    public class FusedLocationProvider : LocationCallback, ILocationListener, ILocationCallbackService
    {
        private readonly ISubject<Location> _locationChanged = new Subject<Location>();
        private readonly ISubject<LocationResult> _locationResultChanged = new Subject<LocationResult>();
        private readonly ISubject<LocationAvailability> _locationAvailability = new Subject<LocationAvailability>();

        public IObservable<LocationResult> LocationChanged => _locationResultChanged.AsObservable();

        public IObservable<LocationAvailability> LocationAvailabilityChanged => _locationAvailability.AsObservable();

        public override void OnLocationAvailability(LocationAvailability locationAvailability) =>
            _locationAvailability.OnNext(locationAvailability);

        public override void OnLocationResult(LocationResult result) => _locationResultChanged.OnNext(result);

        void IDisposable.Dispose()
        {
            _locationChanged.OnCompleted();
            _locationResultChanged.OnCompleted();
            _locationAvailability.OnCompleted();
        }

        public void OnLocationChanged(Location location) => _locationChanged.OnNext(location);
    }
}