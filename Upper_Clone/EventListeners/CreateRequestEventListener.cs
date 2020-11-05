using Firebase.Auth;
using Firebase.Database;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Upper_Clone.DataModels;
using Upper_Clone.Helpers;

namespace Upper_Clone.EventListeners 
{
   public class CreateRequestEventListener : Java.Lang.Object, IValueEventListener
    {
        NewTripDetails newTrip;
        FirebaseDatabase database;
        DatabaseReference newTripRef;
        FirebaseAuth mAuth = FirebaseAuth.Instance;


       // public event EventHandler<DataEventArgs> DataRetrived;
        public class DataEventArgs : EventArgs
        {
            public NewTripDetails fireData { get; set; }
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public CreateRequestEventListener(NewTripDetails mNewTrip)
        {
            newTrip = mNewTrip;
            database = AppDataHelper.GetDatabase();
        }

        public CreateRequestEventListener()
        {
        }

        public void CreateRequest()
        {
            newTripRef = database.GetReference("RiderRequest").Push();

            HashMap location = new HashMap();
            location.Put("latitude", newTrip.PickupLat);
            location.Put("longitude", newTrip.PickupLng);

            HashMap destination = new HashMap();
            destination.Put("latitude", newTrip.DestinationLat);
            destination.Put("longitude", newTrip.DestinationLng);

            HashMap myTrip = new HashMap();

            newTrip.RideID = newTripRef.Key;

            myTrip.Put("rider_id", AppDataHelper.GetCurrentUser().Uid);
            myTrip.Put("location", location);
            myTrip.Put("destination", destination);
            myTrip.Put("destination_address", newTrip.DestinationAddress);
            myTrip.Put("pickup_address", newTrip.PickupAddress);
            myTrip.Put("payment_method", newTrip.Paymentmethod);
            myTrip.Put("created_at", newTrip.Timestamp.ToString());
            myTrip.Put("driver_id", "waiting");
            myTrip.Put("rider_name", AppDataHelper.GetFullName());
            myTrip.Put("rider_phone", AppDataHelper.GetPhone());
            //myTrip.Put("Imgurl",)

            newTripRef.AddValueEventListener(this);
            newTripRef.SetValue(myTrip);


        }

        public void CancelRequest()
        {
            newTripRef.RemoveEventListener(this);
            newTripRef.RemoveValue();
        }


        public string Create()
        {
            DatabaseReference reference = AppDataHelper.GetDatabase().GetReference("users/" + mAuth.CurrentUser.Uid);
          return  reference.AddValueEventListener(this).ToString();
        }

        public void DeleteData(string key)
        {
            DatabaseReference reference = AppDataHelper.GetDatabase().GetReference("users/" + key);
            reference.RemoveValue();
        }
    }
}