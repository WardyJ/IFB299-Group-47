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
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;

namespace zenmc
{
    /// <summary>
    /// Dialog fragment displaying buttons linking to the profile page of each student found
    /// in the search.
    /// </summary>
    class searchName : DialogFragment
    {
        WebClient client = new WebClient();
        private int numResults;
        private string json, studentID;
        bool waiting;
        private Button btnBack;
        public List<Student> studentInfo;
        LinearLayout layout;
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/findstudent.php");
        private NameValueCollection parameters;
        private TextView txtResultsLog;
        private ProgressBar progressBar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            
            var view = inflater.Inflate(Resource.Layout.searchName, container, false);

            btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
            txtResultsLog = view.FindViewById<TextView>(Resource.Id.txtResultsLog);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);

            //Search results returned in the parent activity
            json = Arguments.GetString("searchName");
            studentInfo = JsonConvert.DeserializeObject<List<Student>>(json);

            layout = view.FindViewById<LinearLayout>(Resource.Id.searchLayout);

            addStudents(studentInfo);

            btnBack.Click += btnBack_Click;

            return view;
        }

        /// <summary>
        /// Displays a list of students to the dialog fragment based on the list
        /// of student objects provided. If no objects are in the list, displays a different
        /// message.
        /// </summary>
        /// <param name="students">student objects retrieved from the server</param>
        void addStudents(List<Student> students)
        {
            numResults = 0;
            foreach( Student student in studentInfo)
            {
                Button btnProfile = new Button(Context);
                btnProfile.Text = student.FullName + " ID: " + student.StudentID;
                btnProfile.Id = student.StudentID;
                btnProfile.Click += btnProfile_Click;
                layout.AddView(btnProfile);
                numResults += 1;
            }
            if(numResults < 1)
            {
                txtResultsLog.Text = "No students with that name were found.";
            }
            else { txtResultsLog.Text = numResults + " student(s) were found."; }            
        }

        /// <summary>
        /// /Event that occurs when a student's profile button is clicked. Takes the user
        /// to the profile page of that student.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnProfile_Click(object sender, EventArgs e)
        {
            if (!waiting)
            {
                waiting = true;
                progressBar.Visibility = ViewStates.Visible;

                Button btn = (Button)sender;
                parameters = new NameValueCollection();
                parameters.Add("StudentID", btn.Id.ToString());
                studentID = btn.Id.ToString();

                client.UploadValuesCompleted += client_UploadValuesCompleted;
                client.UploadValuesAsync(uri, parameters);
            }
        }

        /// <summary>
        /// Event that occurs when student info is retrieved from the server. The user is taken
        /// to the profile page of that user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {

            Activity.RunOnUiThread(() =>
            {
                studentID = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                studentID = studentID.Replace("\r", string.Empty).Replace("\n", string.Empty);
                
                var intent = new Intent(Activity, typeof(profile));
                intent.PutExtra("StudentID", studentID);
                StartActivity(intent);
                Dismiss();
            });
        }

        /// <summary>
        /// Event that occurs when back button is clicked, closes the dialog fragment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}