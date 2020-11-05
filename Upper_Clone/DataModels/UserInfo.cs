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

namespace Upper_Clone.DataModels
{
   public class UserInfo
    {

        // Rider Regstrgion Data
        public string RideID { get; set; }
        public string fullname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string ImgURL { get; set; }

    }
}