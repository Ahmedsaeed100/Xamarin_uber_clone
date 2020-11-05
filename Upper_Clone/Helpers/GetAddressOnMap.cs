using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ufinix.Helpers;
using Xamarin.Essentials;

namespace Upper_Clone.Helpers
{
  public class GetAddressOnMap
    {
        string ShowAddress;
        StartLocation startLocation = new StartLocation();
        // Get Lat And Lng By Text Address
        public async Task<StartLocation> GetlatLongbyTextAddress(string address)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync(address);

                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    // double lanandlong = ( location.Latitude + location.Longitude);
                

                    startLocation.lat = location.Latitude;
                    startLocation.lng = location.Longitude;
                    
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
            }

            return startLocation;
        }

        // Get Text Address By Lat And Lng
        public async Task<string> GetAddressByLocation(double lat, double lon) // Get address by Lat and long
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    //var geocodeAddress =
                    //    $"AdminArea:       {placemark.AdminArea}\n" +
                    //    $"CountryCode:     {placemark.CountryCode}\n" +
                    //    $"CountryName:     {placemark.CountryName}\n" +
                    //    $"FeatureName:     {placemark.FeatureName}\n" +
                    //    $"Locality:        {placemark.Locality}\n" +
                    //    $"PostalCode:      {placemark.PostalCode}\n" +
                    //    $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                    //    $"SubLocality:     {placemark.SubLocality}\n" +
                    //    $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                    //    $"Thoroughfare:    {placemark.Thoroughfare}\n";
                    string geocodeAddress = Regex.Replace(placemark.AdminArea.Split()[0], @"[^0-9a-zA-Z\ ]+", "") + " " + placemark.SubAdminArea + " " + placemark.Locality;

                    ShowAddress = (geocodeAddress).ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
            }
            return ShowAddress;
        }

    }
}