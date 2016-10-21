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
using System.Net;
using Newtonsoft.Json;
using Android.Preferences;
using Java.Lang;
using SQLite;
using System.IO;

namespace zenmc
{
    [Activity(Label = "Zen", MainLauncher =true, NoHistory =true, Icon="@drawable/zenicon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashScreen : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/classinfo.php");
        private Uri courseUri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/courseinfo.php");
        private WebClient client = new WebClient();
        private WebClient courseClient = new WebClient();

        private ISharedPreferences pref;
        private ISharedPreferencesEditor editor;

        public List<Class> classInfo;
        public List<Course> courseInfo;

        private Database database = new Database();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.SplashScreen);
            ActionBar.Hide();

            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            editor = pref.Edit();
            editor.Clear();
            editor.Apply();
            
            string folder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Zen");
            Directory.CreateDirectory(folder);
            string databaseFileName = Path.Combine(folder, "Zen.db");
            SQLite3.Config(SQLite3.ConfigOption.Serialized);
            //database.Reset(); //if database changed outside the app.

            database.createDatabase();

            client.DownloadDataCompleted += client_DownloadDataCompleted;
            client.DownloadDataAsync(uri);
            client.Dispose();

        }

        
        
        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string json = Encoding.UTF8.GetString(e.Result);

                editor = pref.Edit();

                editor.PutString("ClassDetails", json);
                editor.Apply();

                organizeClassData();

                courseClient.DownloadDataCompleted += course_DownloadDataCompleted;
                courseClient.DownloadDataAsync(courseUri);
                courseClient.Dispose();
            });
        }

        void course_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string json = Encoding.UTF8.GetString(e.Result);

                editor = pref.Edit();

                editor.PutString("CourseDetails", json);
                editor.Apply();
                editor = pref.Edit();
                organizeCourseData();

                var intent = new Intent(this, typeof(login));
                StartActivity(intent);
            });
        }

        void organizeClassData()
        {
            string json = pref.GetString("ClassDetails", string.Empty);
            editor = pref.Edit();

            classInfo = JsonConvert.DeserializeObject<List<Class>>(json);

            

            foreach (Class session in classInfo)
            {
                database.insertClass(session);
            }
        }

        void organizeCourseData()
        {
            string json = pref.GetString("CourseDetails", string.Empty);
            editor = pref.Edit();

            courseInfo = JsonConvert.DeserializeObject<List<Course>>(json);

            foreach (Course course in courseInfo)
            {
                database.insertCourse(course);
                
                editor.Apply();
            }
        }
    }
}