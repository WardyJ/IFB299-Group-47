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
    [Activity(Label = "Zen Meditation Centre", MainLauncher = false)]
    public class menu : Activity
    {
        private string userID;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            userID = pref.GetString("UserID", string.Empty);
            
            SetContentView(Resource.Layout.menu);
            
            Button profileButton = FindViewById<Button>(Resource.Id.gotoProfileBtn);
            profileButton.Click += (sender, e) =>
            {
                if(userID == "Owner" || userID == "Receptionist")
                {
                    var intent = new Intent(this, typeof(selectProfile));
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(this, typeof(profile));
                    intent.PutExtra("Student", userID);
                    StartActivity(intent);
                }
            };

            Button contactButton = FindViewById<Button>(Resource.Id.gotoContactBtn);
            contactButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(contactPage));
                StartActivity(intent);
            };

            Button donationButton = FindViewById<Button>(Resource.Id.gotoDonationsBtn);
            donationButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(donations));
                StartActivity(intent);
            };

            Button calendarButton = FindViewById<Button>(Resource.Id.gotoCalendarBtn);
            calendarButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(calendar));
                intent.PutExtra("StudentID", userID);
                StartActivity(intent);
            };
            Button smsTestButton = FindViewById<Button>(Resource.Id.gotoSmsTestBtn);
            smsTestButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(smsTest));
                StartActivity(intent);
            };
            Button logOutButton = FindViewById<Button>(Resource.Id.gotoLogOutBtn);
            logOutButton.Click += (sender, e) =>
            {
                //clear stored user data
                ISharedPreferencesEditor editor = pref.Edit();
                editor.Clear();
                editor.Apply();

                //exit to log in screen
                var intent = new Intent(this, typeof(login));
                StartActivity(intent);
            };
            
        }
    }
}