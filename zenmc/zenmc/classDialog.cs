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
    class classDialog : DialogFragment
    {
        private TextView txtType;
        private TextView txtDescription;
        private TextView txtTime;
        private Button btnBack;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.classDialog, container, false);

            txtType = view.FindViewById<TextView>(Resource.Id.txtType);
            txtDescription = view.FindViewById<TextView>(Resource.Id.txtDescription);
            txtTime = view.FindViewById<TextView>(Resource.Id.txtTime);
            btnBack = view.FindViewById<Button>(Resource.Id.btnBack);

            txtType.Text = "Class Type:\n" + Arguments.GetString("Type") + "\n";
            txtDescription.Text = Arguments.GetString("Description");
            txtTime.Text = "Starts: " + Arguments.GetString("Time");
            txtTime.Text += "\nDuration: 2 hours";

            btnBack.Click += btnBack_Click;

            return view;
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}