﻿using System;
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
    [Activity(Label = "Login",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class login : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/login.php");
        private NameValueCollection parameters;
        private ProgressBar progressBar;

        WebClient client = new WebClient();

        private string emailInput;
        private string passwordInput;
        private bool success; //Becomes true when correct login information is given, false if incorrect
        private bool waiting; //Will be true when waiting for the server
        private string userID;
        private Button btnResetPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.login);

            success = false;

            progressBar = FindViewById<ProgressBar>(Resource.Id.loginProgressBar);
            Button loginButton = FindViewById<Button>(Resource.Id.loginBtn);
            loginButton.Click += loginButton_Click;

            btnResetPassword = FindViewById<Button>(Resource.Id.btnResetPassword);
            Button registerButton = FindViewById<Button>(Resource.Id.registerBtn);
            registerButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(registration));
                StartActivity(intent);
            };
            btnResetPassword.Click += btnResetPassword_Click;
        }

        /// <summary>
        /// Event that occurs when login button is clicked. Uploads user input to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            if(!waiting)
            {
                waiting = true;
                emailInput = FindViewById<EditText>(Resource.Id.lgnEmail).Text.Trim();
                passwordInput = FindViewById<EditText>(Resource.Id.lgnPassword).Text.Trim();
                
                parameters = new NameValueCollection();
                parameters.Add("Email", emailInput);
                parameters.Add("Password", passwordInput);

                progressBar.Visibility = ViewStates.Visible;
                client.UploadValuesCompleted += client_UploadValuesCompleted;
                client.UploadValuesAsync(uri, parameters);
                client.Dispose();
            }
        }
        
        /// <summary>
        /// Event that occurs when values have been uploaded to the server. Return value is
        /// examined and if it = None then error text is displayed. Otherwise, the user input
        /// is stored in shared preferences and the user is sent to the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                userID = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                userID = userID.Replace("\r", string.Empty).Replace("\n", string.Empty);

                if (userID != "None")
                {
                    success = true;
                }

                progressBar.Visibility = ViewStates.Invisible;

                if (success == false)
                {
                    FindViewById<TextView>(Resource.Id.loginErrorOutput).Text = "Error: Incorrect email or password.";
                }
                else
                {
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = pref.Edit();

                    editor.Clear();
                    editor.Apply();
                    editor.PutString("UserID", userID);
                    editor.Apply();

                    //Clear old calendar data to update
                    ISharedPreferences enrollmentPref = Application.Context.GetSharedPreferences("EnrollmentInfo", FileCreationMode.Private);
                    editor = enrollmentPref.Edit();
                    editor.Clear();
                    editor.Commit();

                    var intent = new Intent(this, typeof(menu));
                    StartActivity(intent);
                    Finish();
                }
                waiting = false;
            });
        }

        /// <summary>
        /// Event that occurs when reset password button is clicked. The password
        /// reset dialog fragment is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnResetPassword_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            //Show reset password dialog fragment
            passwordReset resetDialog = new passwordReset();
            resetDialog.Show(transaction, "resetDialog");
        }

        /// <summary>
        /// Event that occurs when hardware back button is clicked. Going back is prevented
        /// </summary>
        public override void OnBackPressed()
        {
            //Stop user from leaving the log in page with back button.
            return;
        }
    }
}