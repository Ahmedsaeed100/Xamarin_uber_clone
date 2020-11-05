using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Upper_Clone.Fragments
{
    public class RequestDriver : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler CancelRequest;
        double mfares;
        Button cancelRequestbtn;
        TextView txtfares;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
             View view = inflater.Inflate(Resource.Layout.RequestDriver, container, false);

            cancelRequestbtn = (Button)view.FindViewById(Resource.Id.cancelRequestbtn);
            cancelRequestbtn.Click += CancelRequestbtn_Click;

            txtfares = (TextView)view.FindViewById(Resource.Id.farestxt);
            txtfares.Text = "$" + mfares.ToString();

            return view;
        }

        private void CancelRequestbtn_Click(object sender, EventArgs e)
        {
            CancelRequest?.Invoke(this, new EventArgs());
        }

        public RequestDriver(double fares)
        {
            mfares = fares;
        }

    }
}