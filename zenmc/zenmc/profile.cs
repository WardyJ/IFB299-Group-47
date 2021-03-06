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
using System.Data;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;

namespace zenmc
{
    [Activity(Label = "My Profile", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class profile : Activity
    {
        public List<Student> studentInfo;
        private string studentIDText;

        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/userprofile.php");
        private NameValueCollection parameters = new NameValueCollection();
        
        private Button btnEmergencyContact;
        private Button btnResidentialAddress;
        private Button btnChangePassword;
        private Button btnMenu;
        private Button btnEditProfile;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);
            btnEmergencyContact = FindViewById<Button>(Resource.Id.btnEmergencyContact);
            btnResidentialAddress = FindViewById<Button>(Resource.Id.btnResidentialAddress);
            btnChangePassword = FindViewById<Button>(Resource.Id.btnChangePassword);
            btnMenu = FindViewById<Button>(Resource.Id.btnMenu);
            btnEditProfile = FindViewById<Button>(Resource.Id.btnEditProfile);



            WebClient client = new WebClient();

            
            studentIDText = Intent.GetStringExtra("StudentID");

            parameters.Add("StudentID", studentIDText);

            if ( Intent.HasExtra("Edit"))
            {
                studentInfo = JsonConvert.DeserializeObject<List<Student>>(Intent.GetStringExtra("Edit"));
                studentIDText = studentInfo[0].StudentID.ToString();
                showProfileInfo();
            }
            else
            {
                client.UploadValuesCompleted += client_UploadValuesCompleted;
                client.UploadValuesAsync(uri, parameters);
            }
            
            btnEmergencyContact.Click += btnEmergencyContact_Click;
            btnResidentialAddress.Click += btnResidentialAddress_Click;
            btnChangePassword.Click += btnChangePassword_Click;
            btnMenu.Click += btnMenu_Click;
            btnEditProfile.Click += btnEditProfile_Click;
        }

        /// <summary>
        /// Profile information views are given text based on the related student information.
        /// </summary>
        private void showProfileInfo()
        {
            FindViewById<TextView>(Resource.Id.prfFullName).Text = studentInfo[0].FullName;
            FindViewById<TextView>(Resource.Id.prfDateOfBirth).Text = studentInfo[0].DateOfBirth.ToString("dd/MM/yyyy");
            FindViewById<TextView>(Resource.Id.prfPhoneNumber).Text = studentInfo[0].PhoneNumber;
            FindViewById<TextView>(Resource.Id.prfEmail).Text = studentInfo[0].Email;
            FindViewById<TextView>(Resource.Id.prfGender).Text = studentInfo[0].Gender;
            FindViewById<TextView>(Resource.Id.prfMedical).Text = studentInfo[0].MedicalConditions;
            FindViewById<TextView>(Resource.Id.prfMedication).Text = studentInfo[0].PrescribedMedication;
        }

        /// <summary>
        /// Event that occurs when values are uploaded to the server. The returned json string is
        /// deserialized as a Student class object and assigned to a variable. Method to show
        /// profile is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string json = Encoding.UTF8.GetString(e.Result);
                studentInfo = JsonConvert.DeserializeObject<List<Student>>(json);

                showProfileInfo();
                
            });
        }

        /// <summary>
        /// Event that occurs when emergency contact button is clicked. Emergency contact
        /// info is bundled into a dialog fragment that is then displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmergencyContact_Click(object sender, EventArgs e)
        {
            string contactName = studentInfo[0].ContactName;
            string relationship = studentInfo[0].Relationship;
            string contactPhoneNumber = studentInfo[0].ContactPhoneNumber;

            Bundle contactbundle = new Bundle();
            contactbundle.PutString("contactName", contactName);
            contactbundle.PutString("relationship", relationship);
            contactbundle.PutString("contactPhoneNumber", contactPhoneNumber);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            emergencyContact emergencyDialog = new emergencyContact();
            emergencyDialog.Arguments = contactbundle;
            //Show emergency contact dialog fragment
            emergencyDialog.Show(transaction, "emergencyDialog");
        }

        /// <summary>
        /// Event that occurs when residential address button is clicked. Residential address
        /// info is bundled into a dialog fragment that is then displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResidentialAddress_Click(object sender, EventArgs e)
        {
            string streetAddress = studentInfo[0].StreetAddress;
            string city = studentInfo[0].City;
            string zipOrPostcode = studentInfo[0].ZipOrPostcode;
            string state = studentInfo[0].State;
            string country = studentInfo[0].Country;
            

            Bundle addressbundle = new Bundle();
            addressbundle.PutString("streetAddress", streetAddress);
            addressbundle.PutString("city", city);
            addressbundle.PutString("zipOrPostcode", zipOrPostcode);
            addressbundle.PutString("state", state);
            addressbundle.PutString("country", country);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            residentialAddress addressDialog = new residentialAddress();
            addressDialog.Arguments = addressbundle;
            //Show residential address dialog fragment
            addressDialog.Show(transaction, "addressDialog");
        }

        /// <summary>
        /// Event that occurs when change password button is clicked. Student email
        /// info is bundled into a dialog fragment that is then displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            string email = studentInfo[0].Email;

            Bundle changebundle = new Bundle();
            changebundle.PutString("email", email);
                      
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            //Show change password dialog fragment
            passwordChange changeDialog = new passwordChange();
            changeDialog.Arguments = changebundle;
            changeDialog.Show(transaction, "changeDialog");
        }

        /// <summary>
        /// /Event that occurs when menu button is clicked. Sends the user back to the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(menu));
            intent.PutExtra("studentID", studentIDText);
            StartActivity(intent);
            Finish();
        }

        /// <summary>
        /// Event that occurs when edit profile button is clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(editProfile));
            intent.PutExtra("Info", JsonConvert.SerializeObject(studentInfo));
            StartActivity(intent);
            Finish();
            
        }
    }
}