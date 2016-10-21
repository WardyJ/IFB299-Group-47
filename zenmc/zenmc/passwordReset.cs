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
using Android.Telephony;
using Newtonsoft.Json;

namespace zenmc
{
    class passwordReset : DialogFragment
    {
        private string email;
        private EditText etResetPassword;
        private Button btnResetPassword;
        private ProgressBar progressBar;
        private TextView txtResetError;
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/resetpassword.php");
        private List<Student> studentInfo;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {            
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.passwordReset, container, false);

            etResetPassword = (EditText) view.FindViewById<EditText>(Resource.Id.XetResetPassword);
            btnResetPassword = view.FindViewById<Button>(Resource.Id.btnResetPassword);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.resetPasswordProgressBar);
            txtResetError = view.FindViewById<TextView>(Resource.Id.txtResetError);
            btnResetPassword.Click += btnResetPassword_Click;

            return view;
        }

        void btnResetPassword_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();

            NameValueCollection parameters = new NameValueCollection();
            email = etResetPassword.Text;
            parameters.Add("Email", email);


            progressBar.Visibility = ViewStates.Visible;
            client.UploadValuesCompleted += client_UploadValuesCompleted;
            client.UploadValuesAsync(uri, parameters);
            client.Dispose();
        }
        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {

            Activity.RunOnUiThread(() =>
            {
                string result = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                result = result.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (result == "None")
                {
                    txtResetError.Text = "Error: This email address doesn't belong to a registered account. Please try again.";
                    progressBar.Visibility = ViewStates.Invisible;
                }
                else
                {
                    string json = Encoding.UTF8.GetString(e.Result);
                    studentInfo = JsonConvert.DeserializeObject<List<Student>>(json);
                
                    string password = studentInfo[0].PasswordHash;
                    string PhoneNumber = studentInfo[0].PhoneNumber;

                    progressBar.Visibility = ViewStates.Invisible;
                
                
                    SmsManager.Default.SendTextMessage(PhoneNumber, null, ("Your new temporary password is " + password + "    Please remember to change your password as soon as you log in."), null, null);

                    Dismiss();
                }

            });

        }
    }
}