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
using System.Collections.Specialized;

namespace zenmc
{
    class passwordChange : DialogFragment
    {
        private EditText etOldPassword;
        private EditText etPassword;
        private EditText etConfirmPassword;
        private Button btnBack;
        private Button btnChangePassword;
        private ProgressBar progressBar;
        private TextView txtPasswordError;
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/changepassword.php");
        private string email;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.passwordChange, container, false);

            etOldPassword = (EditText)view.FindViewById<EditText>(Resource.Id.XetOldPassword);
            etPassword = (EditText)view.FindViewById<EditText>(Resource.Id.XetPassword);
            etConfirmPassword = (EditText)view.FindViewById<EditText>(Resource.Id.XetConfirmPassword);
            btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
            btnChangePassword = view.FindViewById<Button>(Resource.Id.btnResetPassword);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.resetPasswordProgressBar);
            txtPasswordError = view.FindViewById<TextView>(Resource.Id.txtPasswordError);

            email = Arguments.GetString("email");

            btnBack.Click += btnBack_Click;
            btnChangePassword.Click += btnChangePassword_Click;

            return view;
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        void btnChangePassword_Click(object sender, EventArgs e)
        {
            bool valid = validateNewPassword();

            if (valid)
            {
                WebClient client = new WebClient();

                 NameValueCollection parameters = new NameValueCollection();
                 string password = etPassword.Text;
                parameters.Add("Email", email);
                 parameters.Add("Password", password);
                parameters.Add("OldPassword", etOldPassword.Text);


                 progressBar.Visibility = ViewStates.Visible;
                 client.UploadValuesCompleted += client_UploadValuesCompleted;
                 client.UploadValuesAsync(uri, parameters);
                 client.Dispose();
            }
            
        }

        bool validateNewPassword()
        {
            if(etOldPassword.Text == "")
            {
                txtPasswordError.Text = "Password change failed failed: You must enter your current password as well as your desired password.";
                return false;
            }
            if(etPassword.Text.Length < 6 )
            {
                txtPasswordError.Text = "Password change failed: Password must be at least 6 characters.";
                return false;
            }
            if (etPassword.Text != etConfirmPassword.Text)
            {
                txtPasswordError.Text = "Password change failed: New password fields don't match";
                return false;
            }
            return true;
        }
        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            Activity.RunOnUiThread(() =>
            {
                string result = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                result = result.Replace("\r", string.Empty).Replace("\n", string.Empty);

                progressBar.Visibility = ViewStates.Invisible;
                if (result == "old")
                {
                    txtPasswordError.Text = "This password is the same as your old one.";
                }
                else if (result == "false")
                {
                    txtPasswordError.Text = "Error: Old password is incorrect.";
                }
                else
                {
                    txtPasswordError.Text = "Password changed succesffully.";
                    btnChangePassword.Visibility = ViewStates.Gone;
                }
            });

        }
    }
}