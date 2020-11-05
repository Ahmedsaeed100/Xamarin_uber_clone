using System;
using System.Net.Http;
using Android.Views;
using System.Threading.Tasks;
using Android.Gms.Maps.Model;
using Newtonsoft.Json;
using yucee.Helpers;
using ufinix.Helpers;
using Android.Gms.Maps;
using Com.Google.Maps.Android;
using Java.Util;
using Android.Graphics;

namespace Upper_Clone.Helpers
{

    // Need Geocoding API With Mony

    //https://stackoverflow.com/questions/3574644/how-can-i-find-the-latitude-and-longitude-from-address جرب الحل دة كدة
    public class MapFunctionHelper
    {
        string mapkey;
        GoogleMap map;
        public double distance;
        public double duration;
        public string distanceString;
        public string durationstring;
        public MapFunctionHelper(string mMapkey, GoogleMap mmap)
        {
            mapkey = mMapkey;
            map = mmap;
        }

        public string GetGeoCodeUrl(double lat, double lng)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&key=" + mapkey;
            return url;
        }

        public async Task<string> GetGeoJsonAsync(string url)
        {
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        public async Task<string> FindCordinateAddress(LatLng position)
        {
            string url = GetGeoCodeUrl(position.Latitude, position.Longitude);
            string json = "";
            string placeAddress = "";

            //Check for Internet connection
            json = await GetGeoJsonAsync(url);

            if (!string.IsNullOrEmpty(json))
            {
                var geoCodeData = JsonConvert.DeserializeObject<GeocodingParser>(json);
                if (!geoCodeData.status.Contains("ZERO"))
                {
                    if (geoCodeData.results[0] != null)
                    {
                        placeAddress = geoCodeData.results[0].formatted_address;
                    }
                }
            }

            return placeAddress;
        }


        public async Task<string> GetDirectionjsonAsync(LatLng Location,LatLng Destination)
        {
            // Origin of Route
            string str_origin = "origin=" + Location.Latitude + "," + Destination.Longitude;

            // Destination of Route
            string str_destination = "destination=" + Destination.Latitude + "," + Destination.Longitude;

            // mode
            string mode = "mode=driving";

            //Buidling the parameters to the webservice
            string parameters = str_origin + "&" + str_destination + "&" + "&"  + mode + "&key=";

            //Output format
            string output = "json";

            string key = "AIzaSyAlnhsIFFUEIHsD1cpHNM94LMsqt6cD_iw";

            //Bulding the final url string
            string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" + parameters + key;

            string json = "";
            json = await GetGeoJsonAsync(url);

            return json;

        }

        public void DrawTripOnMap(string json)
        {
            var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);

            //Decode Encoded Route
            var points = directionData.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(points);

            ArrayList routeList = new ArrayList();
            foreach (LatLng item in line)
            {
                routeList.Add(item);
            }

            //Draw Polylines on Map
            PolylineOptions polylineOptions = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(10)
                .InvokeColor(Color.Teal)
                .InvokeStartCap(new SquareCap())
                .InvokeEndCap(new SquareCap())
                .InvokeJointType(JointType.Round)
                .Geodesic(true);

            Android.Gms.Maps.Model.Polyline mPolyline = map.AddPolyline(polylineOptions);

            //Get first point and lastpoint
            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];

            //Pickup marker options
            MarkerOptions pickupMarkerOptions = new MarkerOptions();
            pickupMarkerOptions.SetPosition(firstpoint);
            pickupMarkerOptions.SetTitle("Pickup Location");
            pickupMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));

            //Destination marker options
            MarkerOptions destinationMarkerOptions = new MarkerOptions();
            destinationMarkerOptions.SetPosition(lastpoint);
            destinationMarkerOptions.SetTitle("Destination");
            destinationMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));

            Marker pickupMarker = map.AddMarker(pickupMarkerOptions);
            Marker destinationMarker = map.AddMarker(destinationMarkerOptions);

            //Get Trip Bounds
            double southlng = directionData.routes[0].bounds.southwest.lng;
            double southlat = directionData.routes[0].bounds.southwest.lat;
            double northlng = directionData.routes[0].bounds.northeast.lng;
            double northlat = directionData.routes[0].bounds.northeast.lat;

            LatLng southwest = new LatLng(southlat, southlng);
            LatLng northeast = new LatLng(northlat, northlng);
            LatLngBounds tripBound = new LatLngBounds(southwest, northeast);

            map.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripBound, 470));
            map.SetPadding(40, 70, 40, 70);
            pickupMarker.ShowInfoWindow();

            duration = directionData.routes[0].legs[0].duration.value;
            distance = directionData.routes[0].legs[0].distance.value;
            durationstring = directionData.routes[0].legs[0].duration.text;
            distanceString = directionData.routes[0].legs[0].distance.text;

        }

        public double EstimateFares()
        {
            double basefare = 20; //USD 
            double distanceFare = 5; //USD per kilometer
            double timefare = 2; //USD per minute

            double kmfares = (distance / 1000) * distanceFare;
            double minsfares = (duration / 60) * timefare;

            double amount = kmfares + minsfares + basefare;
            double fares = Math.Floor(amount / 10) * 10;

            return fares;
        }


        public double StaticEstimateFares()
        {
            System.Random rnd = new System.Random();

            double basefare = 20; //USD 
            double distanceFare = 5; //USD per kilometer
            double timefare = 2; //USD per minute
           
            distance = rnd.Next(1, 90);
            duration = rnd.Next(150,200);

            double kmfares = (distance / 1000) * distanceFare;
            double minsfares = (duration / 60) * timefare;

            double amount = kmfares + minsfares + basefare;
            double fares = Math.Floor(amount / 10) * 10;

            return fares;
        }

    }

}