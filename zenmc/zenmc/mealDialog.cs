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
    /// Dialog fragment displaying information about a meals for the day.
    /// </summary>
    class mealDialog : DialogFragment
    {
        private TextView txtMealHeader, txtMealTime1, txtMealTime2, txtMealTime3, txtMealDetails1,
            txtMealDetails2, txtMealDetails3;
        private Button btnBack;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.mealDialog, container, false);

            txtMealHeader = view.FindViewById<TextView>(Resource.Id.txtMealHeader);
            txtMealTime1 = view.FindViewById<TextView>(Resource.Id.txtMealTime1);
            txtMealTime2 = view.FindViewById<TextView>(Resource.Id.txtMealTime2);
            txtMealTime3 = view.FindViewById<TextView>(Resource.Id.txtMealTime3);
            txtMealDetails1 = view.FindViewById<TextView>(Resource.Id.txtMealDetails1);
            txtMealDetails2 = view.FindViewById<TextView>(Resource.Id.txtMealDetails2);
            txtMealDetails3 = view.FindViewById<TextView>(Resource.Id.txtMealDetails3);
            btnBack = view.FindViewById<Button>(Resource.Id.btnBack);

            txtMealHeader.Text = "Meals for " + Arguments.GetString("Date");
            txtMealTime1.Text = Arguments.GetString("Time1");
            txtMealTime2.Text = Arguments.GetString("Time2");
            txtMealTime3.Text = Arguments.GetString("Time3");
            txtMealDetails1.Text = Arguments.GetString("Details1");
            txtMealDetails2.Text = Arguments.GetString("Details2");
            txtMealDetails3.Text = Arguments.GetString("Details3");

            btnBack.Click += btnBack_Click;

            return view;
        }

        /// <summary>
        /// Closes the dialog fragment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}