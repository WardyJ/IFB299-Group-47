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
using MySql.Data.MySqlClient;
using System.Data;

namespace meditation_centre
{
    [Activity(Label = "My Profile")]
    public class profile : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);
            string studentIDText = Intent.GetStringExtra("studentID");

            //MySqlConnection con = new MySqlConnection("Server=stevie.heliohost.org;Port=3306;database=team47_meditationcentre;User Id=team47;Password=rolav1;charset=utf8");
            MySqlConnection con = new MySqlConnection("Server=sql6.freesqldatabase.com;Port=3306;database=sql6136440;User Id=sql6136440;Password=Q3w7IWjPNS;charset=utf8");

            string userFullName = "";
            DateTime userDateOfBirth = new DateTime();
            string userPhoneNumber = "";
            string userEmail = "";
            string userGender = "";

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM students WHERE StudentID = @studentID", con);
                cmd.Parameters.AddWithValue("@studentID", Int32.Parse(studentIDText));
                cmd.ExecuteNonQuery();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int StudentID = reader.GetInt32(0);
                    userFullName = reader.GetString(1);
                    userDateOfBirth = reader.GetDateTime(2);
                    userPhoneNumber = reader.GetString(3);
                    userEmail = reader.GetString(4);
                    userGender = reader.GetString(6);
                }
                con.Close();
            }


            FindViewById<TextView>(Resource.Id.prfFullName).Text = userFullName;
            FindViewById<TextView>(Resource.Id.prfDateOfBirth).Text = userDateOfBirth.ToString("dd/MM/yyyy");
            FindViewById<TextView>(Resource.Id.prfPhoneNumber).Text = userPhoneNumber;
            FindViewById<TextView>(Resource.Id.prfEmail).Text = userEmail;
            FindViewById<TextView>(Resource.Id.prfGender).Text = userGender;
        }
    }
}