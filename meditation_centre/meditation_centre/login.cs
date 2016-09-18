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
    [Activity(Label = "login", MainLauncher = true)]
    public class login : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);

            Button loginButton = FindViewById<Button>(Resource.Id.loginBtn);
            loginButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(menu));
                StartActivity(intent);
            };

        }
    
    }
}