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
    /// <summary>
    /// Dialog fragment displaying information about a student's emergency contact.
    /// </summary>
    class emergencyContact : DialogFragment
    {        
        private TextView txtContactName;
        private TextView txtRelationship;
        private TextView txtContactPhoneNumber;
        private Button btnBack;
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.emergencyContact, container, false);
            
            txtContactName = view.FindViewById<TextView>(Resource.Id.txtContactName);
            txtRelationship = view.FindViewById<TextView>(Resource.Id.txtRelationship);
            txtContactPhoneNumber = view.FindViewById<TextView>(Resource.Id.txtContactPhoneNumber);
            btnBack = view.FindViewById<Button>(Resource.Id.btnBack);

            txtContactName.Text = Arguments.GetString("contactName");
            txtRelationship.Text = Arguments.GetString("relationship");
            txtContactPhoneNumber.Text = Arguments.GetString("contactPhoneNumber");

            btnBack.Click += btnBack_Click;

            return view;
        }

        /// <summary>
        /// When back button is clicked closes the dialog fragment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}