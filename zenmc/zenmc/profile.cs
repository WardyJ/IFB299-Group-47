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
using System.Data;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;

namespace zenmc
{
    [Activity(Label = "My Profile")]
    public class profile : Activity
    {
        private List<Student> studentInfo;

        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/userprofile.php");
        private NameValueCollection parameters = new NameValueCollection();

        private ProgressBar progressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);
            string studentIDText = Intent.GetStringExtra("studentID");
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);


            WebClient client = new WebClient();

            parameters.Add("StudentID", studentIDText);

            progressBar.Visibility = ViewStates.Visible;
            client.UploadValuesCompleted += client_UploadValuesCompleted;
            client.UploadValuesAsync(uri, parameters);

        }

        private void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string json = Encoding.UTF8.GetString(e.Result);
                studentInfo = JsonConvert.DeserializeObject<List<Student>>(json);

                FindViewById<TextView>(Resource.Id.prfFullName).Text = studentInfo[0].FullName;
                FindViewById<TextView>(Resource.Id.prfDateOfBirth).Text = studentInfo[0].DateOfBirth.ToString("dd/MM/yyyy");
                FindViewById<TextView>(Resource.Id.prfPhoneNumber).Text = studentInfo[0].PhoneNumber;
                FindViewById<TextView>(Resource.Id.prfEmail).Text = studentInfo[0].Email;
                FindViewById<TextView>(Resource.Id.prfGender).Text = studentInfo[0].Gender;

                progressBar.Visibility = ViewStates.Invisible;
            });
        }
    }
}