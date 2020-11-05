using Android;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Java.Util;
using Plugin.Media;
using Refractored.Controls;
using System;
using Upper_Clone.EventListeners;
using Upper_Clone.Helpers;

namespace Upper_Clone.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class RegisterationActivity : AppCompatActivity, IOnSuccessListener, IOnFailureListener,IOnCompleteListener
    {
        TextInputLayout fullNametext;
        TextInputLayout Phonetext;
        TextInputLayout Emailtext;
        TextInputLayout Passwordtext;
        Button registerButton;
        CoordinatorLayout rootView;

        CircleImageView circleImageView;
        byte[] imageArray;

        TextView ClickToLogin;

        FirebaseAuth mAuth;
        FirebaseDatabase database;
        StorageReference storageReference;

        TaskCompletionListener taskCompletionListener = new TaskCompletionListener();

        string fullname, phone, email, password, ImgURL;

        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;

        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            RequestPermissions(permissionGroup, 0);

            InitalizedFirebase();

            mAuth = FirebaseAuth.Instance;
            ConnectControl();
        }

        void InitalizedFirebase()
        {
            var app = FirebaseApp.InitializeApp(this);
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("upper-clone")
                    .SetApiKey("AIzaSyBKOuU-pH_KiLSg4AdWc8AHTSWi-6Jp2NI")
                    .SetDatabaseUrl("https://upper-clone.firebaseio.com")
                    .SetStorageBucket("upper-clone.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(this, options);
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }


        }

        void ConnectControl()
        {
            fullNametext = (TextInputLayout)FindViewById(Resource.Id.FullName);
            Phonetext = (TextInputLayout)FindViewById(Resource.Id.PhoneNu);
            Emailtext = (TextInputLayout)FindViewById(Resource.Id.Email);
            Passwordtext = (TextInputLayout)FindViewById(Resource.Id.Password);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            registerButton = (Button)FindViewById(Resource.Id.btnRegister);

            circleImageView = (CircleImageView)FindViewById(Resource.Id.RegisterImage);

            circleImageView.Click += CircleImageView_Click;

            ClickToLogin = (TextView)FindViewById(Resource.Id.gotologin);

            ClickToLogin.Click += ClickToLogin_Click;

            registerButton.Click += RegisterButton_Click;
        }

        private void CircleImageView_Click(object sender, EventArgs e)
        {
            selectImage_Photo();
        }

        private void ClickToLogin_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
            Finish();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            fullname = fullNametext.EditText.Text;
            phone = Phonetext.EditText.Text;
            email = Emailtext.EditText.Text;
            password = Passwordtext.EditText.Text;

            // Check Is Valid Values
            if (fullname.Length < 3)
            {
                Snackbar.Make(rootView,"please put Full Name",Snackbar.LengthShort).Show();
                return;
            }
            else if (phone.Length < 5)
            {
                Snackbar.Make(rootView, "Please Enter a valid Phone Number", Snackbar.LengthShort).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Snackbar.Make(rootView, "Please Enter a valid Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 4)
            {
                Snackbar.Make(rootView, "Please Enter a password is short than 4 character", Snackbar.LengthShort).Show();
                return;
            }
            else if (imageArray == null)
            {
                Snackbar.Make(rootView, "Please Add personal Image", Snackbar.LengthShort).Show();
                return;
            }

        

            Regsiteruser(email, password);
        }

        void Regsiteruser(string email, string password)
        {



            mAuth.CreateUserWithEmailAndPassword(email, password);

       // Add Rider iamge 
            storageReference = FirebaseStorage.Instance.GetReference("RiderIamges/" + PublicFunctions.generateID());
            storageReference.PutBytes(imageArray)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);

         

        }


        public void OnSuccess(Java.Lang.Object result)
        {
            Snackbar.Make(rootView, "User Registration was Successful", Snackbar.LengthShort).Show();

            if (storageReference != null)
            {
                //  Toast.MakeText(this, "Successful Upload Iamge", ToastLength.Short).Show();

                storageReference.GetDownloadUrl()
                    .AddOnSuccessListener(this);
            }

            if (!string.IsNullOrEmpty(result.ToString()))
            {
                ImgURL = result.ToString();
                // Toast.MakeText(this, ImgURL, ToastLength.Short).Show();
            }
            if (ImgURL.Contains("https"))
            {
                HashMap userMap = new HashMap();
                userMap.Put("fullname", fullname);
                userMap.Put("email", email);
                userMap.Put("phone", phone);
                userMap.Put("ImgURL", ImgURL);

                DatabaseReference userReference = database.GetReference("users/" + mAuth.CurrentUser.Uid);
                userReference.SetValue(userMap);
                SaveToSharedPreference();
                StartActivity(typeof(MainActivity));
                Finish();
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            throw new NotImplementedException();
        }


        public void OnComplete(Task task)
        {
        
         
        }

        void SaveToSharedPreference()
        {
            editor = preferences.Edit();

            editor.PutString("fullname", fullname);
            editor.PutString("email", email);
            editor.PutString("phone", phone);
            editor.PutString("password", password);
            editor.PutString("ImgURL", ImgURL);
            editor.Apply();
        }

        void RetriveData()
        {
            string email = preferences.GetString("email", "");
        }

        // Take A photo From your Camera
        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 40,
                Name = "myimage.jpg",
                Directory = "sample"

            });

            if (file == null)
            {
                return;
            }

            // Convert file to byte array and set the resulting bitmap to imageview
            imageArray = System.IO.File.ReadAllBytes(file.Path);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            circleImageView.SetImageBitmap(bitmap);

        }


        // get an Iamge From Gallrey
        async void selectImage_Photo()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(this, "Upload not supported on this device", ToastLength.Short).Show();
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 40

            });

            // Convert file to byre array, to bitmap and set it to our ImageView

            imageArray = System.IO.File.ReadAllBytes(file.Path);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            circleImageView.SetImageBitmap(bitmap);

        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            //Plugin.Media.StoragePermission storagePermission = new StoragePermission();
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

       
    }
}