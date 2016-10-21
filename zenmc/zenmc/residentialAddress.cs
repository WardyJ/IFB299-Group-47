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

namespace zenmc
{
    class residentialAddress : DialogFragment
    {
        private TextView txtStreetAddress;
        private TextView txtCity;
        private TextView txtZipOrPostcode;
        private TextView txtState;
        private TextView txtCountry;
        private Button btnBack;
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.residentialAddress, container, false);

            txtStreetAddress = view.FindViewById<TextView>(Resource.Id.txtStreetAddress);
            txtCity = view.FindViewById<TextView>(Resource.Id.txtCity);
            txtZipOrPostcode = view.FindViewById<TextView>(Resource.Id.txtZipOrPostcode);
            txtState = view.FindViewById<TextView>(Resource.Id.txtState);
            txtCountry = view.FindViewById<TextView>(Resource.Id.txtCountry);
            btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += btnBack_Click;

            txtStreetAddress.Text = Arguments.GetString("streetAddress");
            txtCity.Text = Arguments.GetString("city");

            string postcode = Arguments.GetString("zipOrPostcode");
            if(postcode == "0")
            {
                txtZipOrPostcode.Text = "";
            }
            else { txtZipOrPostcode.Text = postcode; }
            
            txtState.Text = Arguments.GetString("state");
            txtCountry.Text = Arguments.GetString("country");

            return view;
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}