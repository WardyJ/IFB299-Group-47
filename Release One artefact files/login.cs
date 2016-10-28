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
using System.Security.Cryptography;

namespace meditation_centre
{
    [Activity(Label = "Login", MainLauncher = true)]
    public class login : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string emailInput;
            string passwordInput;

            SetContentView(Resource.Layout.login);

            Button loginButton = FindViewById<Button>(Resource.Id.loginBtn);
            loginButton.Click += (sender, e) =>
            {
                emailInput = FindViewById<EditText>(Resource.Id.lgnEmail).Text;
                passwordInput = FindViewById<EditText>(Resource.Id.lgnPassword).Text;
                //Compute encrypted password to compare with value stored in database
                SHA1 sh1 = SHA1.Create();
                String plaintext = passwordInput;
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                byte[] bytes = encoding.GetBytes(plaintext);
                byte[] result = sh1.ComputeHash(bytes);

                String ciphertext = Convert.ToBase64String(bytes).Replace(@"\", string.Empty);

                //MySqlConnection con = new MySqlConnection("Server=stevie.heliohost.org;Port=3306;database=team47_meditationcentre;User Id=team47;Password=rolav1;charset=utf8");
                MySqlConnection con = new MySqlConnection("Server=sql6.freesqldatabase.com;Port=3306;database=sql6136440;User Id=sql6136440;Password=Q3w7IWjPNS;charset=utf8");

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT StudentID FROM students WHERE Email = @emailInput AND PasswordHash = @passwordInput", con);
                    cmd.Parameters.AddWithValue("@emailInput", emailInput);
                    cmd.Parameters.AddWithValue("@passwordInput", ciphertext);
                    object studentIDObj = cmd.ExecuteScalar();
                    con.Close();
                    if (studentIDObj != null)
                    {
                        int studentID = Convert.ToInt32(studentIDObj);
                        var intent = new Intent(this, typeof(menu));
                        intent.PutExtra("studentID", studentID.ToString());
                        StartActivity(intent);
                    }
                    FindViewById<TextView>(Resource.Id.loginErrorOutput).Text = "Error: Incorrect email or password.";
                }
            };
            Button registerButton = FindViewById<Button>(Resource.Id.registerBtn);
            registerButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(registration));
                StartActivity(intent);
            };

        }
    
    }
}