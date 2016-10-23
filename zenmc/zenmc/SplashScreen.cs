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
        private Uri mealUri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/mealinfo.php");
        private WebClient client = new WebClient();
        private WebClient courseClient = new WebClient();
        private WebClient mealClient = new WebClient();

        private ISharedPreferences pref;
        private ISharedPreferencesEditor editor;

        public List<Class> classInfo;
        public List<Course> courseInfo;
        public List<Meal> mealInfo;

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
            //database.Reset(); if db structure changes

            database.createDatabase();

            client.DownloadDataCompleted += client_DownloadDataCompleted;
            client.DownloadDataAsync(uri);
            client.Dispose();

        }
        
        /// <summary>
        /// Occurs when class details have been downloaded from the server. Methods are called
        /// to organize the class data and download course details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Occurs when course data has been downloaded from the server. A method is called
        /// to organize the course data and the user is sent to the login screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                mealClient.DownloadDataCompleted += meal_DownloadDataCompleted;
                mealClient.DownloadDataAsync(mealUri);
                mealClient.Dispose();

                
            });
        }

        void meal_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string json = Encoding.UTF8.GetString(e.Result);

                editor = pref.Edit();

                editor.PutString("MealDetails", json);
                editor.Apply();
                editor = pref.Edit();
                organizeMealData();

                var intent = new Intent(this, typeof(login));
                StartActivity(intent);
            });
        }

        /// <summary>
        /// The json string returned from the server is deserialized into a list of class objects
        /// which are then inserted into the database
        /// </summary>
        void organizeClassData()
        {
            string json = pref.GetString("ClassDetails", string.Empty);

            classInfo = JsonConvert.DeserializeObject<List<Class>>(json);

            

            foreach (Class session in classInfo)
            {
                database.insertClass(session);
            }
        }

        /// <summary>
        /// The json string returned from the server is deserialized into a list of course objects
        /// which are then inserted into the database
        /// </summary>
        void organizeCourseData()
        {
            string json = pref.GetString("CourseDetails", string.Empty);

            courseInfo = JsonConvert.DeserializeObject<List<Course>>(json);

            foreach (Course course in courseInfo)
            {
                database.insertCourse(course);
            }
        }

        /// <summary>
        /// The json string returned from the server is deserialized into a list of meal objects
        /// which are then inserted into the database
        /// </summary>
        void organizeMealData()
        {
            string json = pref.GetString("MealDetails", string.Empty);

            mealInfo = JsonConvert.DeserializeObject<List<Meal>>(json);

            foreach (Meal meal in mealInfo)
            {
                database.insertMeal(meal);
            }
        }
    }
}