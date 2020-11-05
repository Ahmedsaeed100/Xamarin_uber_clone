using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android;
using Android.Support.V4.App;
using Android.Gms.Location;
using Android.Content;
using Android.Graphics;
using Upper_Clone.Helpers;
using Android.Support.Design.Widget;
using System.Text.RegularExpressions;
using Android.Locations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Upper_Clone.EventListeners;
using Upper_Clone.Fragments;
using Upper_Clone.DataModels;
using System;
using ufinix.Helpers;
using System.Net;

namespace Upper_Clone
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        // Firebase
        UserProfileEventListener ProfileEventListener = new UserProfileEventListener();
        CreateRequestEventListener requestListener;

        //views
        Android.Support.V7.Widget.Toolbar mainToolbar;
        Android.Support.V4.Widget.DrawerLayout drawerLayout;

        //text View
       // TextView UserName;

        //Edit Text
        EditText PickUpLocationtext;
        EditText Destinationtext;

        //Buttons
        Button favouritePlacebtn;
        Button locationSetbtn;
        Button RequestDriverbtn;

        //Radio Buttons
        RadioButton pickupRedio;
        RadioButton DestinationRedio;

        // ImageView
        ImageView CenterMarker;

        //Layouts
        RelativeLayout layoutPickup;
        RelativeLayout layoutDestination;
        // Bottomsheets
        BottomSheetBehavior tripDetailsBottomsheetbehavior;

        // Google maps
        GoogleMap mainMap;

        //Fragments
        RequestDriver requestDriverFragment;

        //DataModels
        NewTripDetails newTripDetails;

        readonly string[] permissionGroupLocation = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        const int requestLocationid = 0;

        LocationRequest mLocationrequest;
        FusedLocationProviderClient LocationClient;
        Android.Locations.Location mLastLocation;
        LocationCallbackHelper mLocationCallback;

        static int UPDATE_ENTERVAL = 5;// 5 second
        static int FASTEST_INTERVAL = 5;
        static int DISPLACEMENT = 1; // DISPLACEMENT = the moving of something from its place or position.

        // Helpers
        GetAddressOnMap GetAddressOnMap = new GetAddressOnMap();

        //Trip Details
        LatLng PickupLocation;
        LatLng DestinationLocation;

        //flag
        int addressRequest = 1;

        // dfsdfsd
        CreateuserListener myuserInfo;

        // Map Helper
        // MapFunctionHelper mapHelper;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            ConnectControl();

            SupportMapFragment mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            CheckLocationPermission();
            CreateLocationRequest();
            GetMyLocation();
            StartLocationUpdate();
            ProfileEventListener.Create();
        }
       
        void ConnectControl()
        {
            // Drawer Layout
            drawerLayout = (Android.Support.V4.Widget.DrawerLayout)FindViewById(Resource.Id.drawerLayout);
            // ToolBar
            mainToolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.mainToolbar);
            SetSupportActionBar(mainToolbar);
            SupportActionBar.Title = "";
            Android.Support.V7.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_menu_action);
            actionBar.SetDisplayHomeAsUpEnabled(true);

            //TextView
            PickUpLocationtext = (EditText)FindViewById(Resource.Id.PickupLocationText);
            Destinationtext = (EditText)FindViewById(Resource.Id.DestinationLocationText);

            // TextView
           // UserName = (TextView)FindViewById(Resource.Id.UserName);
           
            // Radio Buttons
            pickupRedio = (RadioButton)FindViewById(Resource.Id.pickupRedio);
            DestinationRedio = (RadioButton)FindViewById(Resource.Id.DestinationRedio);

            //Buttons
            favouritePlacebtn = (Button)FindViewById(Resource.Id.favouritePlacesButton);
            locationSetbtn = (Button)FindViewById(Resource.Id.locationsetBtn);
            RequestDriverbtn = (Button)FindViewById(Resource.Id.RequestDriverbtn);

            RequestDriverbtn.Click += RequestDriverbtn_Click;

            pickupRedio.Click += PickupRedio_Click;
            DestinationRedio.Click += DestinationRedio_Click;


            favouritePlacebtn.Click += FavouritePlacebtn_Click;
            locationSetbtn.Click += LocationSetbtn_Click;

            // Image View
            CenterMarker = (ImageView)FindViewById(Resource.Id.centerMarker);

            //layoutPickup
            layoutPickup = (RelativeLayout)FindViewById(Resource.Id.layoutPickup);
            layoutDestination = (RelativeLayout)FindViewById(Resource.Id.layoutDestination);

            layoutPickup.Click += LayoutPickUp_Click;
            layoutDestination.Click += LayoutDestination_Click;

            //BottomSheet
            FrameLayout TripDetailsView = (FrameLayout)FindViewById(Resource.Id.tripdetails_bottomsheet);
            tripDetailsBottomsheetbehavior = BottomSheetBehavior.From(TripDetailsView);

            //
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.navView);
            navigationView.SetNavigationItemSelectedListener(null);
            View header = navigationView.GetHeaderView(0);
            TextView name = (TextView)header.FindViewById(Resource.Id.UserName);
            name.Text = AppDataHelper.GetFullName(); // Get From Prefrences

            myuserInfo = new CreateuserListener();
            myuserInfo.Create();
            myuserInfo.DataRetrived += MyuserInfo_DataRetrived;

        }

        private void MyuserInfo_DataRetrived(object sender, CreateuserListener.DataEventArgs e)
        {
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.navView);
            navigationView.SetNavigationItemSelectedListener(null);
            View header = navigationView.GetHeaderView(0);
            TextView name = (TextView)header.FindViewById(Resource.Id.UserName);
            ImageView accountImage = (ImageView)header.FindViewById(Resource.Id.accountImage);
            //Get Data
            name.Text = e.fireData.fullname;


            //// Get imgae URL 
            var imageBitmap = PublicFunctions.GetImageBitmapFromUrl(e.fireData.ImgURL);
            accountImage.SetImageBitmap(imageBitmap);
        }


        #region Click Event Handlers
        private async void PickupRedio_Click(object sender, System.EventArgs e)
        {
            addressRequest = 1;
            pickupRedio.Checked = true;
            DestinationRedio.Checked = false;
            CenterMarker.SetColorFilter(Color.DarkGreen);
            // my home
            //PickupLocation = new LatLng(29.983685, 31.155421);
            StartLocation Pickup_latlongs = await GetAddressOnMap.GetlatLongbyTextAddress(" مصر " + PickUpLocationtext.Text );
            PickupLocation = new LatLng(Pickup_latlongs.lat, Pickup_latlongs.lng);

            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(PickupLocation, 14));

            // Hide Bottom Sheet
            tripDetailsBottomsheetbehavior.State = BottomSheetBehavior.StateHidden;

            // get Address
            //Address address = await ReverseGeocodeCurrentLocation(PickupLocation);
            //DisplayAddress(address);
            //PickUpLocationtext.Text = _addressText;
        }

        private async void DestinationRedio_Click(object sender, System.EventArgs e)
        {
            if (Destinationtext.Text == "")
            {
                Toast.MakeText(this, "Please Add Destination ", ToastLength.Short).Show();
                DestinationRedio.Checked = false;
                return;
            }

            addressRequest = 2;
            pickupRedio.Checked = true;
            DestinationRedio.Checked = true;
            CenterMarker.SetColorFilter(Color.Red);
          

            StartLocation Destination_latlongs = await GetAddressOnMap.GetlatLongbyTextAddress(" مصر " + Destinationtext.Text );
            DestinationLocation = new LatLng(Destination_latlongs.lat, Destination_latlongs.lng);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(DestinationLocation, 14));

            // get Address
            //Address address = await ReverseGeocodeCurrentLocation(DestinationLocation);
            //DisplayAddress(address);
            //Destinationtext.Text = _addressText;

            TripLocationSt();
            // Hide Bottom Sheet
            tripDetailsBottomsheetbehavior.State = BottomSheetBehavior.StateHidden;

        }

        void LocationSetbtn_Click(object sender, System.EventArgs e)
        {
            locationSetbtn.Text = "Please Wait";
            locationSetbtn.Enabled = false;

            TextView txtFare = (TextView)FindViewById(Resource.Id.tripFareText);
            TextView txtTime = (TextView)FindViewById(Resource.Id.TripTime);
            txtFare.Text = "$ 150 - 200";
            txtTime.Text = "35 min";

            //string json;
            //json = await mapHelper.GetDirectionjsonAsync(PickupLocation, DestinationLocation);

            //if (!string.IsNullOrEmpty(json))
            //{

            //   // mapHelper.DrawTripOnMap(json);
            //    tripDetailsBottomsheetbehavior.State = BottomSheetBehavior.StateExpanded;
            //}

            locationSetbtn.Text = "Done";
            locationSetbtn.Enabled = true;
            tripDetailsBottomsheetbehavior.State = BottomSheetBehavior.StateExpanded;


        }

        private void FavouritePlacebtn_Click(object sender, System.EventArgs e)
        {
            TripLocationSt();
        }

        private void LayoutPickUp_Click(object sender, System.EventArgs e)
        {
         
        }

        private void LayoutDestination_Click(object sender, System.EventArgs e)
        {
           

            //Intent intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeOverlay)
            //    .Build(this);

            //StartActivityForResult(intent, 1);
        }

        private void RequestDriverbtn_Click(object sender, System.EventArgs e)
        {
            requestDriverFragment = new RequestDriver(150);
            requestDriverFragment.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            requestDriverFragment.Show(trans, "request");
            requestDriverFragment.CancelRequest += RequestDriverFragment_CancelRequest;



            newTripDetails = new NewTripDetails();
            newTripDetails.DestinationAddress = Destinationtext.Text;
            newTripDetails.PickupAddress = PickUpLocationtext.Text;

            newTripDetails.PickupLat = PickupLocation.Latitude;
            newTripDetails.PickupLng = PickupLocation.Longitude;

            newTripDetails.DestinationLat = DestinationLocation.Latitude;
            newTripDetails.DestinationLng = DestinationLocation.Longitude;
            newTripDetails.DistanceString = "25";
            newTripDetails.DistanceValue = 150;
            newTripDetails.DurationString = "ss";
            newTripDetails.DurationValue = 33;
            newTripDetails.EstimateFare = 150;
            newTripDetails.Paymentmethod = "cash";
            newTripDetails.Timestamp = DateTime.Now;

            requestListener = new CreateRequestEventListener(newTripDetails);
            requestListener.CreateRequest();
        }

        private void RequestDriverFragment_CancelRequest(object sender, EventArgs e)
        {
            //User cancel before driver accepts a  trip
            if (requestDriverFragment != null && requestListener != null)
            {
                requestListener.CancelRequest();
                requestListener = null;

                requestDriverFragment.Dismiss();
                requestDriverFragment = null;
            }

        }

        #endregion


        #region Trip Config

        void TripLocationSt()
        {
            favouritePlacebtn.Visibility = ViewStates.Invisible;
            locationSetbtn.Visibility = ViewStates.Visible;
        }

        #endregion


        #region Maps And Location Services
        public void OnMapReady(GoogleMap googleMap)
        {

            try
            {
                bool success = googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.MymapStyle));
            }
            catch (System.Exception)
            {
            }
            mainMap = googleMap;

            mainMap.CameraIdle += MainMap_CameraIdle;
            //string mapkey = Resources.GetString(Resource.String.MapKey);
            //mapHelper = new MapFunctionHelper(mapkey, mainMap);

        }

        private void MainMap_CameraIdle(object sender, System.EventArgs e)
        {
            if (addressRequest == 1)
            {
                PickupLocation = mainMap.CameraPosition.Target;
               // PickUpLocationtext.Text = "Al haram";
            }
            else if (addressRequest == 2)
            {
                DestinationLocation = mainMap.CameraPosition.Target;
              //  Destinationtext.Text = "Sheikh Zayed City";
            }
        }

        // Create Location Request
        void CreateLocationRequest()
        {
            mLocationrequest = new LocationRequest();
            mLocationrequest.SetInterval(UPDATE_ENTERVAL);
            mLocationrequest.SetFastestInterval(FASTEST_INTERVAL);
            mLocationrequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mLocationrequest.SetSmallestDisplacement(DISPLACEMENT);
            LocationClient = LocationServices.GetFusedLocationProviderClient(this);
            mLocationCallback = new LocationCallbackHelper();
            mLocationCallback.MyLocation += MLocationCallback_MyLocation;
        }

       async void MLocationCallback_MyLocation(object sender , LocationCallbackHelper.OnlocationCapturedEventArgs e)
        {
            mLastLocation = e.Location;
            LatLng MyPosition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(MyPosition, 12));
            PickUpLocationtext.Text = await GetAddressOnMap.GetAddressByLocation(MyPosition.Latitude, MyPosition.Longitude);
            //PickUpLocationtext.Text = sss.ToString();
        }

        void StartLocationUpdate()
        {
            if (CheckLocationPermission())
            {
                LocationClient.RequestLocationUpdates(mLocationrequest, mLocationCallback, null);
            }
        }

        void StopLocationUpdates()
        {
            if (LocationClient != null && mLocationCallback != null)
            {
                LocationClient.RemoveLocationUpdates(mLocationCallback);
            }
        }

        // get My Location
        async void GetMyLocation()
        {
            if (!CheckLocationPermission())
            {
                return;
            }

            mLastLocation = await LocationClient.GetLastLocationAsync();
            if (mLastLocation != null)
            {
                LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 20));

                // get Address
                Address address = await ReverseGeocodeCurrentLocation(myposition);
                DisplayAddress(address);

                MarkerOptions options = new MarkerOptions()

               .SetPosition(myposition)
               .SetIcon(null)
          //  .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.greenmarker))
          .SetTitle(Governorate)
          .SetSnippet(_addressText)
          .Draggable(true);

                mainMap.AddMarker(options);
            }
        }
        #endregion


        // Check Permissions
        bool CheckLocationPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                permissionGranted = false;
                RequestPermissions(permissionGroupLocation, requestLocationid);
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }

        #region Override Methods

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            if (grantResults.Length == (int)Android.Content.PM.Permission.Granted)
            {
                Toast.MakeText(this, "Permisson Was Granted", ToastLength.Short).Show();
                GetMyLocation();
            }
            else
            {
                Toast.MakeText(this, "Permisson Was denied", ToastLength.Short).Show();
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1)
            {
                // my home
                LatLng PickupPosition = new LatLng(29.983685, 31.155421);
                mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(PickupPosition, 15));
               
            }

            if (requestCode == 2)
            {
                // Destention 30.042618, 30.981538
                LatLng DestentionPosition = new LatLng(30.042618, 30.981538);
                mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(DestentionPosition, 15));
              
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        #endregion


        // Get Address 
        //vars
        string _addressText;
        string Governorate;
     
        async Task<Address> ReverseGeocodeCurrentLocation(LatLng latLng)
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }
        //"Ezbet Mohsen Al Sadaf, Al Qerdahi, Qism El-Montaza, Alexandria Governorate, Egypt"],
        void DisplayAddress(Address address)
        {
            if (address != null)
            {

                string s = address.ToString();
                Regex f = new Regex(@"""(.+?),");
                MatchCollection mc1 = f.Matches(s);
                _addressText = mc1[0].Groups[1].Value;

                Regex r = new Regex(@",(.+?),");
                MatchCollection mc = r.Matches(s);
                _addressText += mc[0].Groups[1].Value;
                Governorate = mc[1].Groups[1].Value;


                //StringBuilder deviceAddress = new StringBuilder();
                //for (int i = 0; i < address.MaxAddressLineIndex; i++)
                //{
                //    deviceAddress.Append(address.GetAddressLine(i));
                //}
                //// Remove the last comma from the end of the address.
                //_addressText = deviceAddress.ToString();
            }
            else
            {
                _addressText = "Unable to determine the address. Try again in a few minutes.";
            }
        }


    }
}
