using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Upper_Clone.Helpers;

namespace Upper_Clone.EventListeners
{
    public class UserProfileEventListener : Java.Lang.Object, IValueEventListener
    {
        ISharedPreferences Preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor Editor;
        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                string fullname, email, phone;
                fullname = (snapshot.Child("fullname") != null) ? snapshot.Child("fullname").Value.ToString() : "";
                email = (snapshot.Child("email") != null) ? snapshot.Child("email").Value.ToString() : "";
                phone = (snapshot.Child("phone") != null) ? snapshot.Child("phone").Value.ToString() : "";

                Editor.PutString("fullname", fullname);
                Editor.PutString("email", email);
                Editor.PutString("phone", phone);
                Editor.Apply();
            }
        }

        public void Create()
        {
            Editor = Preferences.Edit();
            FirebaseDatabase database = AppDataHelper.GetDatabase();
            string userId = AppDataHelper.GetCurrentUser().Uid;
            DatabaseReference ProfileReference = database.GetReference("users/" + userId);
            ProfileReference.AddValueEventListener(this);
        }

    }
}