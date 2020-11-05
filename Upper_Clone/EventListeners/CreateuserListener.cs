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
   public class CreateuserListener : Java.Lang.Object, IValueEventListener
    {
        FirebaseAuth mAuth = FirebaseAuth.Instance;
        UserInfo UserInfo;
        FirebaseDatabase database;

        public event EventHandler<DataEventArgs> DataRetrived;
        public class DataEventArgs : EventArgs
        {
            public UserInfo fireData { get; set; }
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
               // var child = snapshot.Children.ToEnumerable<DataSnapshot>();

                var Info = snapshot.Child("");
                UserInfo userinfo = new UserInfo();

                userinfo.RideID = Info.Key;
                userinfo.fullname = Info.Child("fullname").Value.ToString();
                userinfo.phone = Info.Child("phone").Value.ToString();
                userinfo.email = Info.Child("email").Value.ToString();
                userinfo.ImgURL = Info.Child("ImgURL").Value.ToString();

                DataRetrived.Invoke(this, new DataEventArgs { fireData = userinfo });
            }
        }

        public void OnCancelled(DatabaseError error)
        {

        }

     

        public CreateuserListener(UserInfo myuserInfo)
        {
            UserInfo = myuserInfo;
            database = AppDataHelper.GetDatabase();
        }

        public CreateuserListener()
        {
        }

        public void CancelRequest()
        {
          
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