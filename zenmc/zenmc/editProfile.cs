using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Telephony;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Specialized;
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace zenmc
{
    [Activity(Label = "Edit Profile", Icon = "@drawable/icon")]
    public class editProfile : Activity
    {
        public List<Student> studentInfo;
        //All user input will be assigned to these variables
        private EditText etFullName, etEmail, etPhoneNumber, etContactName,
            etRelationship, etContactPhoneNumber, etMedicalConditions, etPrescribedMedication,
            etStreetAddress, etCity, etZipOrPostcode, etState, etCountry;
        private TextView displayDate;
        private Button btnDate;
        private int day, month, year;
        private RadioButton btnGender;
        private String gender;
        private String birthday;
        private Button btnUpdate;
        private Button btnCancel;



        //Text output to show users errors they have made
        private TextView errorLog;

        //Address of web application used to insert new student info into database
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/updatestudent.php");
        //Collection of parameters with student info to be uploaded to the web application
        private NameValueCollection parameters;

        private ProgressBar progressBar;

        //Used to remember if any errors have been picked up during form validation
        private Boolean errors;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.editProfile);

            //Assigning to variables all possible user input
            etFullName = FindViewById<EditText>(Resource.Id.XetFullName);
            etEmail = FindViewById<EditText>(Resource.Id.XetEmail);
            btnDate = FindViewById<Button>(Resource.Id.btnDate);
            btnUpdate = FindViewById<Button>(Resource.Id.btnUpdate);
            btnCancel = FindViewById<Button>(Resource.Id.XbtnCancel);
            

            etPhoneNumber = FindViewById<EditText>(Resource.Id.XetPhoneNumber);
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
            errorLog = FindViewById<TextView>(Resource.Id.XerrorLog);
            displayDate = FindViewById<TextView>(Resource.Id.displayDateOfBirth);

            progressBar = FindViewById<ProgressBar>(Resource.Id.editProfileProgressBar);

            if (Intent.HasExtra("Student"))
            {
                studentInfo = JsonConvert.DeserializeObject<List<Student>>(Intent.GetStringExtra("Student"));
            }
            else
            {
                studentInfo = JsonConvert.DeserializeObject<List<Student>>(Intent.GetStringExtra("Owner"));
            }
            

            etFullName.Text = studentInfo[0].FullName;
            etEmail.Text = studentInfo[0].Email;
            displayDate.Text = studentInfo[0].DateOfBirth.ToString("dd/MM/yyyy");
            birthday = studentInfo[0].DateOfBirth.ToString("yyyy-MM-dd");
            etPhoneNumber.Text = studentInfo[0].PhoneNumber;
            if(studentInfo[0].Gender == "Male")
            {
                btnGender.Selected = true;
            }
            else
            {
                btnGender.Selected = false;
                FindViewById<RadioButton>(Resource.Id.XetFemale).Selected = true;
            }
            etContactName.Text = studentInfo[0].ContactName;
            etRelationship.Text = studentInfo[0].Relationship;
            etContactPhoneNumber.Text = studentInfo[0].ContactPhoneNumber;
            etMedicalConditions.Text = studentInfo[0].MedicalConditions;
            etPrescribedMedication.Text = studentInfo[0].PrescribedMedication;
            etStreetAddress.Text = studentInfo[0].StreetAddress;
            etCity.Text = studentInfo[0].City;
            if (studentInfo[0].ZipOrPostcode == "0")
            {
                etZipOrPostcode.Text = "";
            }
            else { etZipOrPostcode.Text = studentInfo[0].ZipOrPostcode; }
            etState.Text = studentInfo[0].State;
            etCountry.Text = studentInfo[0].Country;
            
            btnDate = FindViewById<Button>(Resource.Id.btnDate);
            btnDate.Click += btnDate_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void btnDate_Click(object sender, EventArgs e)
        {
            DialogDate dialogfrag = DialogDate.NewInstance(delegate (DateTime date)
            {
                birthday = date.ToString("dd/MM/yyyy");
                displayDate.Text = birthday;
                day = date.Day;
                month = date.Month;
                year = date.Year;
                birthday = year + "-" + month + "-" + day;
            });
            dialogfrag.Show(FragmentManager, DialogDate.tag);
        }

        /// <summary>
        /// Called whenever the Update button is clicked. Checks if there are errors in user
        /// input and if there are none attempts to add updated student information to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (btnGender.Checked)
            {
                gender = "Male";
            }
            else
            {
                gender = "Female";
            }

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
            etContactName.Background = GetDrawable(Resource.Drawable.EditText);
            etContactPhoneNumber.Background = GetDrawable(Resource.Drawable.EditText);
            btnDate.SetTextColor(Android.Graphics.Color.SkyBlue);            
        }

        /// <summary>
        /// Calls all validation methods to validate the edit profile form.
        /// </summary>
        void validateForm()
        {
            validateFullName();
            validateEmail();
            validatePhoneNumber();
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
                //FindViewById<TextView>(Resource.Id.TextFullName).SetTextColor(Android.Graphics.Color.Red);
            }
            Regex rgx = new Regex(@"^[a-zA-Z -']+$");
            if (!rgx.IsMatch(etFullName.Text))
            {
                errorLog.Text += "Error: Only letters and a few special characters are allowed in name\n";
                errors = true;
                //FindViewById<TextView>(Resource.Id.TextFullName).SetTextColor(Android.Graphics.Color.Red);
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
                //FindViewById<TextView>(Resource.Id.TextEmail).SetTextColor(Android.Graphics.Color.Red);
                etEmail.Background = GetDrawable(Resource.Drawable.EditTextError);
                errors = true;
            }
            if (etEmail.Text == "")
            {
                errorLog.Text += "Error: Email is a required field.\n";
                //FindViewById<TextView>(Resource.Id.TextEmail).SetTextColor(Android.Graphics.Color.Red);
                etEmail.Background = GetDrawable(Resource.Drawable.EditTextError);
                errors = true;
            }
        }

        void validatePhoneNumber()
        {
            if (etPhoneNumber.Text == "")
            {
                errorLog.Text += "Error: Phone number is a required field";
                errors = true;
                etPhoneNumber.Background = GetDrawable(Resource.Drawable.EditTextError);
                //FindViewById<TextView>(Resource.Id.TextPhoneNumber).SetTextColor(Android.Graphics.Color.Red);
            }
        }

        
        void validateContactName()
        {
            Regex rgx = new Regex(@"^[a-zA-Z -']+$");

            if (!rgx.IsMatch(etContactName.Text))
            {
                errorLog.Text += "Error: Only letters and a few special characters are allowed in contact name\n";
                errors = true;
                etContactName.Background = GetDrawable(Resource.Drawable.EditTextError);
                //FindViewById<TextView>(Resource.Id.TextContactName).SetTextColor(Android.Graphics.Color.Red);
            }
            if (etContactName.Text == "")
            {
                errorLog.Text += "Error: Emergency contact name is a required field.\n";
                errors = true;
                etContactName.Background = GetDrawable(Resource.Drawable.EditTextError);
                //FindViewById<TextView>(Resource.Id.TextContactName).SetTextColor(Android.Graphics.Color.Red);
            }
        }

        void validateContactPhoneNumber()
        {
            if (etContactPhoneNumber.Text == "")
            {
                errorLog.Text += "Error: Emergency contact phone number is a required field.\n";
                errors = true;
                etContactPhoneNumber.Background = GetDrawable(Resource.Drawable.EditTextError);
                //FindViewById<TextView>(Resource.Id.TextContactPhoneNumber).SetTextColor(Android.Graphics.Color.Red);
            }

        }

        void setParameters()
        {
            parameters = new NameValueCollection();
            parameters.Add("StudentID", studentInfo[0].StudentID.ToString());
            parameters.Add("FullName", etFullName.Text);
            parameters.Add("DateOfBirth", birthday);
            parameters.Add("PhoneNumber", etPhoneNumber.Text);
            parameters.Add("Email", etEmail.Text);
            parameters.Add("OldEmail", studentInfo[0].Email);
            parameters.Add("Gender", gender);
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

        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {

            RunOnUiThread(() =>
            {
                string returnValue = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                returnValue = returnValue.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (returnValue == "duplicate")
                {
                    errorLog.Text = "The email you tried to change to is already in use.";
                }
                else
                {
                    string json = Encoding.UTF8.GetString(e.Result);
                    studentInfo = JsonConvert.DeserializeObject<List<Student>>(json);


                    progressBar.Visibility = ViewStates.Invisible;

                    Intent intent = new Intent(this, typeof(profile));

                    if (Intent.HasExtra("Student"))
                    {
                        intent.PutExtra("StudentEdit", JsonConvert.SerializeObject(studentInfo));
                    }
                    else
                    {
                        intent.PutExtra("OwnerEdit", JsonConvert.SerializeObject(studentInfo));
                    }
                    StartActivity(intent);
                    Finish();
                }
                progressBar.Visibility = ViewStates.Invisible;
            });
        }
        void btnCancel_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(profile));

            if (Intent.HasExtra("Student"))
            {
                intent.PutExtra("StudentEdit", JsonConvert.SerializeObject(studentInfo));
            }
            else
            {
                intent.PutExtra("OwnerEdit", JsonConvert.SerializeObject(studentInfo));
            }

            StartActivity(intent);
            Finish();
        }
    }
}

