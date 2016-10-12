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
    [Activity(Label = "Select Profile")]
    public class selectProfile : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/findstudent.php");
        private NameValueCollection parameters;

        WebClient client = new WebClient();
        public List<Student> studentInfo;
        private int searchCount;

        private EditText etSelectID, etSelectEmail, etSearchName;
        private Button btnSelectID, btnSelectEmail, btnSearchName;
        private TextView txtErrorLog;
        private ProgressBar progressBar;
        private string studentID;
        private bool waiting;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.selectProfile);

            etSelectID = FindViewById<EditText>(Resource.Id.XetSelectID);
            btnSelectID = FindViewById<Button>(Resource.Id.btnSelectID);
            etSelectEmail = FindViewById<EditText>(Resource.Id.XetSelectEmail);
            btnSelectEmail = FindViewById<Button>(Resource.Id.btnSelectEmail);
            etSearchName = FindViewById<EditText>(Resource.Id.XetSearchName);
            btnSearchName = FindViewById<Button>(Resource.Id.btnSearchName);
            txtErrorLog = FindViewById<TextView>(Resource.Id.txtErrorLog);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            
            btnSelectID.Click += btnSelectID_Click;
            btnSelectEmail.Click += btnSelectEmail_Click;
            btnSearchName.Click += btnSearchName_Click;
        }

        void btnSelectID_Click(object sender, EventArgs e)
        {
            if (!waiting)
            {
                searchCount = 0;
                waiting = true;
                if (etSelectID.Text == "")
                {
                    waiting = false;
                }
                else
                {
                    parameters = new NameValueCollection();
                    parameters.Add("StudentID", etSelectID.Text);
                    progressBar.Visibility = ViewStates.Visible;
                    client.UploadValuesCompleted += client_UploadValuesCompleted;
                    client.UploadValuesAsync(uri, parameters);
                }
            }
        }

        void btnSelectEmail_Click(object sender, EventArgs e)
        {
            if (!waiting)
            {
                searchCount = 0;
                waiting = true;
                if (etSelectEmail.Text == "")
                {
                    waiting = false;
                }
                else
                {
                    parameters = new NameValueCollection();
                    parameters.Add("Email", etSelectEmail.Text);
                    progressBar.Visibility = ViewStates.Visible;

                    client.UploadValuesCompleted += client_UploadValuesCompleted;
                    client.UploadValuesAsync(uri, parameters);
                }
            }
        }

        void btnSearchName_Click(object sender, EventArgs e)
        {
            if (!waiting)
            {
                searchCount = 0;
                waiting = true;
                if (etSearchName.Text == "")
                {
                    waiting = false;
                }
                else
                {
                    parameters = new NameValueCollection();
                    parameters.Add("FullName", etSearchName.Text);
                    progressBar.Visibility = ViewStates.Visible;

                    client.UploadValuesCompleted += search_UploadValuesCompleted;
                    client.UploadValuesAsync(uri, parameters);
                }
            }
        }

        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            if (searchCount < 1)
            {
                searchCount += 1;
                RunOnUiThread(() =>
                {
                    if (e.Result != null)
                    {
                        studentID = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                        studentID = studentID.Replace("\r", string.Empty).Replace("\n", string.Empty);

                        if (studentID == "None")
                        {
                            txtErrorLog.Text = "Error: There is no student with this ID or Email.";
                        }
                        else
                        {
                            var intent = new Intent(this, typeof(profile));
                            intent.PutExtra("Owner", studentID);
                            StartActivity(intent);
                        }
                        waiting = false;
                        progressBar.Visibility = ViewStates.Invisible;
                    }
                });
            }
        }

        void search_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            if(searchCount < 1)
            {
                searchCount += 1;
                RunOnUiThread(() =>
                {
                    if (e.Result != null)
                    {
                        string json = Encoding.UTF8.GetString(e.Result);
                        Bundle searchbundle = new Bundle();
                        searchbundle.PutString("searchName", json);


                        FragmentTransaction transaction = FragmentManager.BeginTransaction();
                        searchName searchDialog = new searchName();
                        searchDialog.Arguments = searchbundle;
                        waiting = false;
                        progressBar.Visibility = ViewStates.Invisible;
                        searchDialog.Show(transaction, "searchDialog");
                    }
                });
            }
        }
    }
}