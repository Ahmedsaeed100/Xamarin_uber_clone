using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Upper_Clone.EventListeners;
using Upper_Clone.Helpers;

namespace Upper_Clone.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class LoginActivity : AppCompatActivity
    {
        TextInputLayout emailText;
        TextInputLayout passwordText;
        Button loginButton;
        CoordinatorLayout rootView;
        FirebaseAuth mAuth;

        TextView ClickToRegistertxt;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            emailText = (TextInputLayout)FindViewById(Resource.Id.Email);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.Password);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            loginButton = (Button)FindViewById(Resource.Id.btnLogin);

            loginButton.Click += LoginButton_Click;


            ClickToRegistertxt = (TextView)FindViewById(Resource.Id.gotoregister);

            ClickToRegistertxt.Click += ClickToRegistertxt_Click;

        }
        private void ClickToRegistertxt_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegisterationActivity));
            Finish();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            mAuth = AppDataHelper.GetFirebaseAuth();
            string email, password;

            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;

            if (!email.Contains("@"))
            {
                Snackbar.Make(rootView, "Please Provide a valid Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 3)
            {
                Snackbar.Make(rootView, "Please Provide a valid Password", Snackbar.LengthShort).Show();
                return;
            }

            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            mAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(taskCompletionListener)
                .AddOnFailureListener(taskCompletionListener);
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "Login Failed", Snackbar.LengthShort).Show();
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
            Finish();
        }


    }
}