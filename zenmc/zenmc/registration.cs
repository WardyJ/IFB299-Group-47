using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Data;
using System.Security.Cryptography;
using Android.Telephony;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Globalization;

namespace zenmc
{
    [Activity(Label = "Registration", Icon = "@drawable/zenicon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class registration : Activity
    {
        //All user input will be assigned to these variables
        private EditText etFullName, etEmail, etPhoneNumber, etPassword, etConfirmPassword, etContactName,
            etRelationship, etContactPhoneNumber, etMedicalConditions, etPrescribedMedication,
            etStreetAddress, etCity, etZipOrPostcode, etState, etCountry;
        private TextView displayDate;
        private Button btnDate;
        private int day, month, year;
        private RadioButton btnGender;
        private String gender;
        private String birthday = null;
        private Button btnInsert;



        //Text output to show users errors they have made
        private TextView errorLog;

        //Address of web application used to insert new student info into database
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/insertstudent.php");
        //Collection of parameters with student info to be uploaded to the web application
        private NameValueCollection parameters;

        private ProgressBar progressBar;

        //Used to remember if any errors have been picked up during form validation
        private Boolean errors;

        //value of new student ID if succesfully registered, or set to "None" if unsuccesful
        private String newID;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.registration);

            //Assigning to variables all possible user input
            etFullName = FindViewById<EditText>(Resource.Id.XetFullName);
            etEmail = FindViewById<EditText>(Resource.Id.XetEmail);
            btnDate = FindViewById<Button>(Resource.Id.btnDate);

            etPhoneNumber = FindViewById<EditText>(Resource.Id.XetPhoneNumber);
            etPassword = FindViewById<EditText>(Resource.Id.XetPassword);
            etConfirmPassword = FindViewById<EditText>(Resource.Id.XetConfirmPassword);
            btnGender = FindViewById<RadioButton>(Resource.Id.XetMale);
            etContactName = FindViewById<EditText>(Resource.Id.XetContactName);
            etRelationship = FindViewById<EditText>(Resource.Id.XetRelationship);
            etContactPhoneNumber = FindViewById<EditText>(Resource.Id.XetContactPhoneNumber);
            etMedicalConditions = FindViewById<EditText>(Resource.Id.XetMedicalConditions);
            etPrescribedMedication = FindViewById<EditText>(Resource.Id.XetPrescribedMedication);
            etStreetAddress = FindViewById<EditText>(Resource.Id.XetStreetAddress);
            etCity = FindViewById<EditText>(Resource.Id.XetCity);
            etZipOrPostcode = FindViewById<EditText>(Resource.Id.XetZipOrPostcode);
            etState = FindViewById<EditText>(Resource.Id.XetState);
            etCountry = FindViewById<EditText>(Resource.Id.XetCountry);
            btnInsert = FindViewById<Button>(Resource.Id.XbtnInsert);
            errorLog = FindViewById<TextView>(Resource.Id.XerrorLog);


            progressBar = FindViewById<ProgressBar>(Resource.Id.registerProgressBar);

            displayDate = FindViewById<TextView>(Resource.Id.displayDateOfBirth);
            btnDate = FindViewById<Button>(Resource.Id.btnDate);
            btnDate.Click += btnDate_Click;
            btnInsert.Click += btnInsert_Click;
        }

        /// <summary>
        /// Event called when the set date button is clicked. Opens up a date picker dialog
        /// fragment for user to input their date of birth.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDate_Click(object sender, EventArgs e)
        {
            DialogDate dialogfrag = DialogDate.NewInstance(delegate (DateTime date)
            {
                birthday = date.ToString("dd/MM/yyyy");
                displayDate.Text = birthday;
                day = date.Day;
                month = date.Month;
                year = date.Year;
                
            });
            dialogfrag.Show(FragmentManager, DialogDate.tag);
        }

        /// <summary>
        /// Called whenever the Register button is clicked. Checks if there are errors in user
        /// input and if there are none attempts to add new student information to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (btnGender.Checked)
            {
                gender = "Male";
            }
            else
            {
                gender = "Female";
            }

            birthday = year + "-" + month + "-" + day;

            errors = false;
            resetErrorText();
            validateForm();

            if (!errors)
            {
                WebClient client = new WebClient();

                setParameters();
                

                progressBar.Visibility = ViewStates.Visible;
                client.UploadValuesCompleted += client_UploadValuesCompleted;
                client.UploadValuesAsync(uri, parameters);
                client.Dispose();
            }
        }

        /// <summary>
        /// Removes all error text and resets all form fields to neutral white text.
        /// </summary>
        void resetErrorText()
        {
            errorLog.Text = "";
            etFullName.Background = GetDrawable(Resource.Drawable.EditText);
            etEmail.Background = GetDrawable(Resource.Drawable.EditText);
            etPhoneNumber.Background = GetDrawable(Resource.Drawable.EditText);
            etPassword.Background = GetDrawable(Resource.Drawable.EditText);
            etConfirmPassword.Background = GetDrawable(Resource.Drawable.EditText);
            etContactName.Background = GetDrawable(Resource.Drawable.EditText);
            etContactPhoneNumber.Background = GetDrawable(Resource.Drawable.EditText);
            btnDate.SetTextColor(Android.Graphics.Color.SkyBlue);
        }

        /// <summary>
        /// Calls all validation methods to validate the registration form.
        /// </summary>
        void validateForm()
        {
            validateFullName();
            validateEmail();
            validateDateOfBirth();
            validatePhoneNumber();
            validatePassword();
            validateContactName();
            validateContactPhoneNumber();            
        }

        /// <summary>
        /// Checks if FullName field is blank or has unacceptable characters, if so assigns errors to true
        /// </summary>
        void validateFullName()
        {
            if (etFullName.Text == "")
            {
                errorLog.Text += "Error: Name is a required field.\n";
                errors = true;
                etFullName.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
            Regex rgx = new Regex(@"^[a-zA-Z -']+$");
            if (!rgx.IsMatch(etFullName.Text))
            {
                errorLog.Text += "Error: Only letters and a few special characters are allowed in name\n";
                errors = true;
                etFullName.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
        }

        /// <summary>
        /// Checks if email field is set and is of proper form. If not, errors is set to true.
        /// </summary>
        void validateEmail()
        {
            Regex rgx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!rgx.IsMatch(etEmail.Text))
            {
                errorLog.Text += "Error: Email must be of the form address@email.com\n";
                etEmail.Background = GetDrawable(Resource.Drawable.EditTextError);
                errors = true;
            }
            if (etEmail.Text == "")
            {
                errorLog.Text += "Error: Email is a required field.\n";
                etEmail.Background = GetDrawable(Resource.Drawable.EditTextError);
                errors = true;
            }
        }

        /// <summary>
        /// Checks if date of birth value has been changed. If not, displays error text.
        /// </summary>
        void validateDateOfBirth()
        {
            if (birthday == "0-0-0")
            {
                errors = true;
                errorLog.Text += "Error: Date of birth must be set.\n";
                btnDate.SetTextColor(Android.Graphics.Color.Red);
            }            
        }

        /// <summary>
        /// Checks input phone number, if blank displays error text
        /// </summary>
        void validatePhoneNumber()
        {
            if (etPhoneNumber.Text == "")
            {
                errorLog.Text += "Error: Phone number is a required field";
                errors = true;
                etPhoneNumber.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
        }

        /// <summary>
        /// Checks password fields. If invalid, display error message.
        /// </summary>
        void validatePassword()
        {
            if (etPassword.Text == "")
            {
                errorLog.Text += "Error: Password is a required field.\n";
                errors = true;
                etPassword.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
            if (etPassword.Text != etConfirmPassword.Text)
            {
                errorLog.Text += "Error: Password fields don't match.\n";
                errors = true;
                etPassword.Background = GetDrawable(Resource.Drawable.EditTextError);
                etConfirmPassword.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
            if (etPassword.Text.Length < 6)
            {
                errorLog.Text += "Error: Password must be at least 6 characters long\n";
                errors = true;
                etPassword.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
        }

        /// <summary>
        /// Checks if the emergency contact name provided is valid, displays error message if not
        /// </summary>
        void validateContactName()
        {
            Regex rgx = new Regex(@"^[a-zA-Z -']+$");

            if (!rgx.IsMatch(etContactName.Text))
            {
                errorLog.Text += "Error: Only letters and a few special characters are allowed in contact name\n";
                errors = true;
                etContactName.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
            if (etContactName.Text == "")
            {
                errorLog.Text += "Error: Emergency contact name is a required field.\n";
                errors = true;
                etContactName.Background = GetDrawable(Resource.Drawable.EditTextError);
            }
        }

        /// <summary>
        /// Checks if emergency contact phone number provided is valid, displays error text if not
        /// </summary>
        void validateContactPhoneNumber()
        {
            if (etContactPhoneNumber.Text == "")
            {
                errorLog.Text += "Error: Emergency contact phone number is a required field.\n";
                errors = true;
                etContactPhoneNumber.Background = GetDrawable(Resource.Drawable.EditTextError);
            }

        }

        /// <summary>
        /// Sets parameters from user input to be uploaded when user tries to register
        /// </summary>
        void setParameters()
        {
            parameters = new NameValueCollection();
            parameters.Add("FullName", etFullName.Text);
            parameters.Add("DateOfBirth", birthday);
            parameters.Add("PhoneNumber", etPhoneNumber.Text);
            parameters.Add("Email", etEmail.Text);
            parameters.Add("Gender", gender);
            parameters.Add("Password", etPassword.Text);
            parameters.Add("MedicalConditions", etMedicalConditions.Text);
            parameters.Add("PrescribedMedication", etPrescribedMedication.Text);
            parameters.Add("ContactName", etContactName.Text);
            parameters.Add("Relationship", etRelationship.Text);
            parameters.Add("ContactPhoneNumber", etContactPhoneNumber.Text);
            parameters.Add("StreetAddress", etStreetAddress.Text);
            parameters.Add("City", etCity.Text);
            parameters.Add("ZipOrPostcode", etZipOrPostcode.Text);
            parameters.Add("State", etState.Text);
            parameters.Add("Country", etCountry.Text);
        }

        /// <summary>
        /// Event that occurs when user registration details are uploaded. If an error string
        /// is returned, an error message is displayed. Otherwise, the user is sent back to the
        /// menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {

            RunOnUiThread(() =>
            {

                newID = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                newID = newID.Replace("\r", string.Empty).Replace("\n", string.Empty);

                progressBar.Visibility = ViewStates.Invisible;
                if (newID == "duplicate")
                {
                    errorLog.Text = "This email is already in use.";
                }
                else
                {
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = pref.Edit();
                    editor.PutString("UserID", newID);
                    editor.Apply();
                    

                    SmsManager.Default.SendTextMessage(etPhoneNumber.Text, null, ("Confirmation message to " + etFullName.Text), null, null);

                    var intent = new Intent(this, typeof(menu));
                    StartActivity(intent);
                    Finish();

                }

            });

        }
    }
}

