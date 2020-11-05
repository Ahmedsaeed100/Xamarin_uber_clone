using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Gms.Location;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace Upper_Clone.Helpers
{
   public class LocationCallbackHelper : LocationCallback
    {
        public EventHandler<OnlocationCapturedEventArgs> MyLocation;
        public class OnlocationCapturedEventArgs : EventArgs
        {
            public Android.Locations.Location Location { get; set; }
        }
        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            Log.Debug("Car Bus", "islocationAvailable : {0}", locationAvailability.IsLocationAvailable);
        }

        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Count !=0)
            {
                MyLocation?.Invoke(this, new OnlocationCapturedEventArgs { Location = result.Locations[0] });
            }
        }

    }
}