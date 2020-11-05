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
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace Upper_Clone.Helpers
{
   public static class AppDataHelper
    {
        static ISharedPreferences Preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);

        //InitalizedFirebase
        public static FirebaseDatabase GetDatabase()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase database;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("upper-clone")
                    .SetApiKey("AIzaSyBKOuU-pH_KiLSg4AdWc8AHTSWi-6Jp2NI")
                    .SetDatabaseUrl("https://upper-clone.firebaseio.com")
                    .SetStorageBucket("upper-clone.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

            return database;
        }

        public static FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("upper-clone")
                    .SetApiKey("AIzaSyBKOuU-pH_KiLSg4AdWc8AHTSWi-6Jp2NI")
                    .SetDatabaseUrl("https://upper-clone.firebaseio.com")
                    .SetStorageBucket("upper-clone.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }

            return mAuth;
        }


        public static FirebaseUser GetCurrentUser()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;
            FirebaseUser mUser;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("upper-clone")
                    .SetApiKey("AIzaSyBKOuU-pH_KiLSg4AdWc8AHTSWi-6Jp2NI")
                    .SetDatabaseUrl("https://upper-clone.firebaseio.com")
                    .SetStorageBucket("upper-clone.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;
                mUser = mAuth.CurrentUser;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
                mUser = mAuth.CurrentUser;
            }

            return mUser;
        }


        public static string GetFullName()
        {
            string fullname = Preferences.GetString("fullname", "");
            return fullname;
        }

        public static string GetEmail()
        {
            string email = Preferences.GetString("email", "");
            return email;
        }

        public static string GetPhone()
        {
            string phone = Preferences.GetString("phone", "");
            return phone;
        }

        public static string GetImgURL()
        {
            string phone = Preferences.GetString("ImgURL", "");
            return phone;
        }

    }
}