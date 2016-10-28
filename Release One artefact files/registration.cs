using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography;
using Android.Telephony;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meditation_centre
{
    [Activity(Label = "Registration", Icon = "@drawable/icon")]
    public class registration : Activity
    {

        private EditText etFullName, etEmail, etDay, etMonth, etYear, etPhoneNumber, etPassword, etConfirmPassword, etContactName,
            etRelationship, etContactPhoneNumber, etMedicalCondition, etPrescribedMedication,
            etStreetAddress, etCity, etZipOrPostcode, etState, etCountry;
        private RadioButton btnGender;
        private String gender;
        private String date;
        private Button btnInsert;
        private TextView txtSyslog;

        private Boolean errors;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.registration);

            etFullName = FindViewById<EditText>(Resource.Id.XetFullName);
            etEmail = FindViewById<EditText>(Resource.Id.XetEmail);
            etDay = FindViewById<EditText>(Resource.Id.XetDay);
            etMonth = FindViewById<EditText>(Resource.Id.XetMonth);
            etYear = FindViewById<EditText>(Resource.Id.XetYear);
            etPhoneNumber = FindViewById<EditText>(Resource.Id.XetPhoneNumber);
            etPassword = FindViewById<EditText>(Resource.Id.XetPassword);
            etConfirmPassword = FindViewById<EditText>(Resource.Id.XetConfirmPassword);
            btnGender = FindViewById<RadioButton>(Resource.Id.XetMale);
            etContactName = FindViewById<EditText>(Resource.Id.XetContactName);
            etRelationship = FindViewById<EditText>(Resource.Id.XetRelationship);
            etContactPhoneNumber = FindViewById<EditText>(Resource.Id.XetContactPhoneNumber);
            etMedicalCondition = FindViewById<EditText>(Resource.Id.XetMedicalCondition);
            etPrescribedMedication = FindViewById<EditText>(Resource.Id.XetPrescribedMedication);
            etStreetAddress = FindViewById<EditText>(Resource.Id.XetStreetAddress);
            etCity = FindViewById<EditText>(Resource.Id.XetCity);
            etZipOrPostcode = FindViewById<EditText>(Resource.Id.XetZipOrPostcode);
            etState = FindViewById<EditText>(Resource.Id.XetState);
            etCountry = FindViewById<EditText>(Resource.Id.XetCountry);
            btnInsert = FindViewById<Button>(Resource.Id.XbtnInsert);
            txtSyslog = FindViewById<TextView>(Resource.Id.XtxtSysLog);


            btnInsert.Click += BtnInsert_Click;
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection("Server=sql6.freesqldatabase.com;Port=3306;database=sql6136440;User Id=sql6136440;Password=Q3w7IWjPNS;charset=utf8");
            //MySqlConnection con = new MySqlConnection("Server=stevie.heliohost.org;Port=3306;database=team47_meditationcentre;User Id=team47;Password=rolav1;charset=utf8");
            //MySqlConnection con = new MySqlConnection("Server=127.0.0.1;Port=3306;database=meditationcentre;User Id=team47;Password=rolav1;charset=utf8");

            if (btnGender.Checked)
            {
                gender = "Male";
            }
            else
            {
                gender = "Female";
            }

            date = etDay.Text + "/" + etMonth.Text + "/" + etYear.Text;

            SHA1 sh1 = SHA1.Create();
            String plaintext = etPassword.Text;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(plaintext);
            byte[] result = sh1.ComputeHash(bytes);

            String ciphertext = Convert.ToBase64String(bytes).Replace(@"\", string.Empty);

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    errors = false;

                    txtSyslog.Text = "";

                    FindViewById<TextView>(Resource.Id.TextFullName).SetTextColor(Android.Graphics.Color.White);
                    FindViewById<TextView>(Resource.Id.TextDateOfBirth).SetTextColor(Android.Graphics.Color.White);
                    FindViewById<TextView>(Resource.Id.TextPhoneNumber).SetTextColor(Android.Graphics.Color.White);
                    FindViewById<TextView>(Resource.Id.TextPassword).SetTextColor(Android.Graphics.Color.White);
                    FindViewById<TextView>(Resource.Id.TextConfirmPassword).SetTextColor(Android.Graphics.Color.White);
                    FindViewById<TextView>(Resource.Id.TextContactName).SetTextColor(Android.Graphics.Color.White);
                    FindViewById<TextView>(Resource.Id.TextContactPhoneNumber).SetTextColor(Android.Graphics.Color.White);
                    FindViewById<TextView>(Resource.Id.TextPhoneNumber).SetTextColor(Android.Graphics.Color.White);

                    if (etFullName.Text == "")
                    {
                        txtSyslog.Text += "Error: Name is a required field.\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextFullName).SetTextColor(Android.Graphics.Color.Red);
                    }
                    Regex rgx = new Regex(@"^[a-zA-Z -']+$");
                    if (!rgx.IsMatch(etFullName.Text))
                    {
                        txtSyslog.Text += "Error: Only letters and a few special characters are allowed in name\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextFullName).SetTextColor(Android.Graphics.Color.Red);
                    }
                    if (!rgx.IsMatch(etContactName.Text))
                    {
                        txtSyslog.Text += "Error: Only letters and a few special characters are allowed in contact name\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextContactName).SetTextColor(Android.Graphics.Color.Red);
                    }
                    if (etContactName.Text == "")
                    {
                        txtSyslog.Text += "Error: Emergency contact name is a required field.\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextContactName).SetTextColor(Android.Graphics.Color.Red);
                    }
                    if (etContactPhoneNumber.Text == "")
                    {
                        txtSyslog.Text += "Error: Emergency contact phone number is a required field.\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextContactPhoneNumber).SetTextColor(Android.Graphics.Color.Red);
                    }
                    if (etPhoneNumber.Text == "")
                    {
                        txtSyslog.Text += "Error: Phone number is a required field";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextPhoneNumber).SetTextColor(Android.Graphics.Color.Red);
                    }
                    if (etPassword.Text == "")
                    {
                        txtSyslog.Text += "Error: Password is a required field.\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextPassword).SetTextColor(Android.Graphics.Color.Red);
                    }
                    if (etPassword.Text != etConfirmPassword.Text)
                    {
                        txtSyslog.Text += "Error: Password fields don't match.\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextPassword).SetTextColor(Android.Graphics.Color.Red);
                        FindViewById<TextView>(Resource.Id.TextConfirmPassword).SetTextColor(Android.Graphics.Color.Red);
                    }
                    if (etPassword.Text.Length < 6)
                    {
                        txtSyslog.Text += "Error: Password must be at least 6 characters long\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextPassword).SetTextColor(Android.Graphics.Color.Red);
                    }
                    rgx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    if (!rgx.IsMatch(etEmail.Text))
                    {
                        txtSyslog.Text += "Error: Email must be of the form address@email.com\n";
                        FindViewById<TextView>(Resource.Id.TextEmail).SetTextColor(Android.Graphics.Color.Red);
                        errors = true;
                    }
                    if (etEmail.Text == "")
                    {
                        txtSyslog.Text += "Error: Email is a required field.\n";
                        FindViewById<TextView>(Resource.Id.TextEmail).SetTextColor(Android.Graphics.Color.Red);
                        errors = true;

                    }
                    if (etDay.Text == "" || etMonth.Text == "" || etYear.Text == "")
                    {
                        txtSyslog.Text += "Error: Date of Birth is missing input\n";
                        errors = true;
                        FindViewById<TextView>(Resource.Id.TextDateOfBirth).SetTextColor(Android.Graphics.Color.Red);
                    }
                    else
                    {
                        Regex dayrgx = new Regex(@"^[0-9]{1,2}$");
                        Regex yearrgx = new Regex(@"^[0-9]{4}$");
                        if (!dayrgx.IsMatch(etDay.Text) || !dayrgx.IsMatch(etMonth.Text) || !yearrgx.IsMatch(etYear.Text))
                        {
                            FindViewById<TextView>(Resource.Id.TextDateOfBirth).SetTextColor(Android.Graphics.Color.Red);
                            errors = true;
                            txtSyslog.Text += "Error: Date of Birth must be numericals characters with form dd mm yyyy (day month year)";
                        }
                        else
                        {
                            int[] monthLength = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                            int year = 0;
                            int day = 0;
                            int month = 0;
                            Int32.TryParse(etYear.Text, out year);
                            Int32.TryParse(etDay.Text, out day);
                            Int32.TryParse(etMonth.Text, out month);
                            if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0))
                            {
                                monthLength[1] = 29;
                            }
                            if (month > 12 || month < 1 || day < 1 || day > monthLength[month - 1])
                            {
                                txtSyslog.Text += "Error: Date given for date of birth does not exist.\n";
                                errors = true;
                                FindViewById<TextView>(Resource.Id.TextDateOfBirth).SetTextColor(Android.Graphics.Color.Red);
                            }
                        }

                    }
                    if (!errors)
                    {
                        con.Open();
                        //txtSyslog.Text = "Successfully connected";
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO students(FullName, DateOfBirth, PhoneNumber, Email, Gender, PasswordHash) VALUES (@FullName,@DateOfBirth,@PhoneNumber,@Email,@Gender,@PasswordHash)", con);
                        cmd.Parameters.AddWithValue("@FullName", etFullName.Text);
                        cmd.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(date));
                        cmd.Parameters.AddWithValue("@PhoneNumber", etPhoneNumber.Text);
                        cmd.Parameters.AddWithValue("@Email", etEmail.Text);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@PasswordHash", ciphertext);
                        cmd.ExecuteNonQuery();

                        cmd = new MySqlCommand("SELECT max(StudentID) FROM students", con);
                        int studentID = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd = new MySqlCommand("INSERT INTO emergencycontact(StudentID,ContactName,Relationship,PhoneNumber) VALUES (@StudentID, @ContactName, @Relationship, @PhoneNumber)", con);
                        cmd.Parameters.AddWithValue("@studentID", studentID);
                        cmd.Parameters.AddWithValue("@ContactName", etContactName.Text);
                        cmd.Parameters.AddWithValue("@Relationship", etRelationship.Text);
                        cmd.Parameters.AddWithValue("@PhoneNumber", etContactPhoneNumber.Text);
                        cmd.ExecuteNonQuery();

                        cmd = new MySqlCommand("INSERT INTO medicalinformation(StudentID,MedicalCondition,PrescribedMedication) VALUES (@StudentID, @MedicalCondition, @PrescribedMedication)", con);
                        cmd.Parameters.AddWithValue("@studentID", studentID);
                        cmd.Parameters.AddWithValue("@MedicalCondition", etMedicalCondition.Text);
                        cmd.Parameters.AddWithValue("@PrescribedMedication", etPrescribedMedication.Text);
                        cmd.ExecuteNonQuery();

                        cmd = new MySqlCommand("INSERT INTO residentialaddress(StudentID,StreetAddress,City,ZipOrPostcode,State,Country) VALUES (@StudentId, @StreetAddress, @City, @ZipOrPostcode, @State, @Country)", con);
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@StreetAddress", etStreetAddress.Text);
                        cmd.Parameters.AddWithValue("@City", etCity.Text);
                        cmd.Parameters.AddWithValue("@ZipOrPostcode", etZipOrPostcode.Text);
                        cmd.Parameters.AddWithValue("@State", etState.Text);
                        cmd.Parameters.AddWithValue("@Country", etCountry.Text);
                        cmd.ExecuteNonQuery();

                        SmsManager.Default.SendTextMessage(etPhoneNumber.Text, null, ("Confirmation message to " + etFullName.Text), null, null);
                        {
                            var intent = new Intent(this, typeof(menu));
                            intent.PutExtra("studentID", studentID.ToString());
                            StartActivity(intent);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                txtSyslog.Text = ex.ToString();
            }
            finally
            {
                con.Close();
            }
        }
    }
}

