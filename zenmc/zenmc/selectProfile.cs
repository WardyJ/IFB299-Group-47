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
    [Activity(Label = "Select Profile", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class selectProfile : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/findstudent.php");
        private NameValueCollection parameters;

        WebClient client = new WebClient();
        public List<Student> studentInfo;
        private int idCount;
        private int emailCount;
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

        /// <summary>
        /// Occurs when select by ID button is clicked. If user input is provided, uploads the
        /// input to the server to find student information matching the ID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSelectID_Click(object sender, EventArgs e)
        {
            if (!waiting)
            {
                txtErrorLog.Text = "";
                idCount = 0;
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
                    client.UploadValuesCompleted += id_UploadValuesCompleted;
                    client.UploadValuesAsync(uri, parameters);
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// Occurs when select by Email button is clicked. If user input is provided, uploads the
        /// input to the server to find student information matching the Email.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSelectEmail_Click(object sender, EventArgs e)
        {
            if (!waiting)
            {
                txtErrorLog.Text = "";
                emailCount = 0;
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

                    client.UploadValuesCompleted += email_UploadValuesCompleted;
                    client.UploadValuesAsync(uri, parameters);
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// Occurs when search by name button is clicked. If user input is provided, uploads the
        /// input to the server to find all students with the input in their name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSearchName_Click(object sender, EventArgs e)
        {
            if (!waiting)
            {
                txtErrorLog.Text = "";
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
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// Occurs when an ID value has been uploaded to the server. Displays error text if 
        /// server found no student with that ID otherwise takes user to the student's profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void id_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            if (idCount < 1)//If no other results have been returned (prevents multiple events occuring when this page is returned to)
            {
                idCount += 1;
                RunOnUiThread(() =>
                {
                    try
                    {
                        if (e.Result != null)
                        {
                            studentID = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                            studentID = studentID.Replace("\r", string.Empty).Replace("\n", string.Empty);

                            if (studentID == "None")
                            {
                                txtErrorLog.Text = "Error: There is no student with this ID.";
                            }
                            else
                            {
                                var intent = new Intent(this, typeof(profile));
                                intent.PutExtra("StudentID", studentID);
                                StartActivity(intent);
                            }
                            waiting = false;
                            progressBar.Visibility = ViewStates.Invisible;
                        }
                    }
                    catch (System.Reflection.TargetInvocationException)
                    {
                        ;
                    }
                });
            }
        }

        /// <summary>
        /// Occurs when an Email value has been uploaded to the server. Displays error text if 
        /// server found no student with that Email otherwise takes user to the student's profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void email_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            if (emailCount < 1)
            {
                emailCount += 1;
                RunOnUiThread(() =>
                {
                    if (e.Result != null)
                    {
                        studentID = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                        studentID = studentID.Replace("\r", string.Empty).Replace("\n", string.Empty);

                        if (studentID == "None")
                        {
                            txtErrorLog.Text = "Error: There is no student with this email address.";
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

        /// <summary>
        /// Occurs when a name value has been uploaded to the server. A dialog fragment is 
        /// opened with information on any and all students found with the name value in their
        /// name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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