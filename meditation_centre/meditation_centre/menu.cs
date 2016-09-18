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

namespace meditation_centre
{
    [Activity(Label = "Zen Meditation Centre", MainLauncher = false)]
    public class menu : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.menu);

            Button profileButton = FindViewById<Button>(Resource.Id.gotoProfileBtn);
            profileButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(profile));
                StartActivity(intent);
            };

            Button contactButton = FindViewById<Button>(Resource.Id.gotoContactBtn);
            contactButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(contactPage));
                //intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
                StartActivity(intent);
            };

            Button donationButton = FindViewById<Button>(Resource.Id.gotoDonationsBtn);
            donationButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(donations));
                //intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
                StartActivity(intent);
            };

            Button calendarButton = FindViewById<Button>(Resource.Id.gotoCalendarBtn);
            calendarButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(calendar));
                //intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
                StartActivity(intent);
            };
            Button smsTestButton = FindViewById<Button>(Resource.Id.gotoSmsTestBtn);
            smsTestButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(smsTest));
                //intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
                StartActivity(intent);
            };
            // Create your application here
        }
    }
}