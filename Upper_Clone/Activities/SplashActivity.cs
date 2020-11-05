using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Firebase.Auth;
using System;
using System.IO;
using System.Threading;
using Upper_Clone.Helpers;

namespace Upper_Clone.Activities
{
    [Activity(Label = "@strings/app_name", Theme = "@style/MyTheme.Splash", MainLauncher =true,NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
          
            // Create your application here
        }


        protected override void OnResume()
        {
            base.OnResume();
            FirebaseUser currentuser = AppDataHelper.GetCurrentUser();
            if (currentuser == null)
            {
                StartActivity(typeof(LoginActivity));
                Finish();
            }
            else
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }

        }
    }
}