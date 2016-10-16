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
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Net;

namespace zenmc
{
    [Activity(Label = "Course Info")]
    public class course : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/login.php");
        private NameValueCollection parameters;
        private ProgressBar progressBar;
        private TextView txtCourseName, txtDescription, txtCommencement;

        WebClient client = new WebClient();

        
        private Button btnBack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.course);

            txtCourseName = FindViewById<TextView>(Resource.Id.txtCourseName);
            txtDescription = FindViewById<TextView>(Resource.Id.txtDescription);


            btnBack = FindViewById<Button>(Resource.Id.btnBack);

            btnBack.Click += btnBack_Click;

        }

        void btnBack_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(calendar));
            StartActivity(intent);
        }
    }
}