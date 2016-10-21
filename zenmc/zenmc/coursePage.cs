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
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Net;
using Android.Preferences;

namespace zenmc
{
    [Activity(Label = "Course Info", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class coursePage : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/courseregister.php");
        private Uri unregisterUri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/courseunregister.php");
        private NameValueCollection parameters;
        private ProgressBar progressBar;
        private TextView txtCourseName, txtDescription, txtCourseRequirements, txtMaleStudents, txtFemaleStudents, txtMaleManagers, txtFemaleManagers,
            txtMaleTAs, txtFemaleTAs, txtKitchenHelp, txtDate, txtLength, txtCourseHistory, errorLog;
        private string courseID, studentID, role;
        private bool btnDescriptionPressed, btnRegisterPressed;
        private RadioGroup rdoRole;
        private RadioButton btnStudent, btnManager, btnAssistantTeacher, btnKitchenHelp;

        string courseLength, commencementDate, numMaleStudents, numFemaleStudents, maleManager,
            femaleManager, maleTA, femaleTA, numKitchenHelp;
        
        WebClient client = new WebClient();

        private EditText etCourseBG1;
        private EditText etCourseBG2;
        private EditText etCourseBG3;
        private EditText etCourseBG4;
        private EditText etCourseBG5;

        private Database database;

        private Button btnDescription, btnBack, btnRegister, btnConfirmRegister, btnUnregister;
        private ISharedPreferences pref, enrollmentPref, userPref;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.coursePage);

            database = new Database();

            courseID = Intent.GetStringExtra("CourseID");
            courseLength = database.getCourseDetail(courseID, "Length");
            commencementDate = database.getCourseDetail(courseID, "CommencementDate");
            numMaleStudents = database.getCourseDetail(courseID, "MaleStudents");
            numFemaleStudents = database.getCourseDetail(courseID, "FemaleStudents");
            maleManager = database.getCourseDetail(courseID, "MaleManagers") != "0" ? "Yes" : "Needed";
            femaleManager = database.getCourseDetail(courseID, "FemaleManagers") != "0" ? "Yes" : "Needed";
            maleTA = database.getCourseDetail(courseID, "MaleTAs") != "0" ? "Yes" : "Needed";
            femaleTA = database.getCourseDetail(courseID, "FemaleTAs") != "0" ? "Yes" : "Needed";
            numKitchenHelp = database.getCourseDetail(courseID, "KitchenHelp");



            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            enrollmentPref = Application.Context.GetSharedPreferences("EnrollmentInfo", FileCreationMode.Private);
            userPref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);

            txtCourseName = FindViewById<TextView>(Resource.Id.txtCourseName);
            txtDescription = FindViewById<TextView>(Resource.Id.txtDescription);
            txtCourseRequirements = FindViewById<TextView>(Resource.Id.txtCourseRequirements);
            txtDate = FindViewById<TextView>(Resource.Id.txtDate);
            txtLength = FindViewById<TextView>(Resource.Id.txtLength);
            txtCourseRequirements = FindViewById<TextView>(Resource.Id.txtCourseRequirements);
            txtMaleStudents = FindViewById<TextView>(Resource.Id.txtMaleStudents);
            txtFemaleStudents = FindViewById<TextView>(Resource.Id.txtFemaleStudents);
            txtMaleManagers = FindViewById<TextView>(Resource.Id.txtMaleManagers);
            txtFemaleManagers = FindViewById<TextView>(Resource.Id.txtFemaleManagers);
            txtMaleTAs = FindViewById<TextView>(Resource.Id.txtMaleTAs);
            txtFemaleTAs = FindViewById<TextView>(Resource.Id.txtFemaleTAs);
            txtKitchenHelp = FindViewById<TextView>(Resource.Id.txtKitchenHelp);
            txtCourseHistory = FindViewById<TextView>(Resource.Id.txtCourseHistory);
            errorLog = FindViewById<TextView>(Resource.Id.registerErrorOutput);

            txtCourseName.Text = database.getCourseDetail(courseID, "CourseName");
            txtDescription.Text = database.getCourseDetail(courseID, "Description");
            txtDate.Text = "Course Begins: " + DateTime.Parse(commencementDate).ToString("dd/MM/yyyy");
            txtLength.Text = "Course Length: " + courseLength + " Days";
            txtMaleStudents.Text += numMaleStudents + "/26";
            txtFemaleStudents.Text += numFemaleStudents + "/26\n";
            txtMaleManagers.Text += maleManager;
            txtFemaleManagers.Text += femaleManager + "\n";
            txtMaleTAs.Text += maleTA;
            txtFemaleTAs.Text += femaleTA + "\n";
            txtKitchenHelp.Text += numKitchenHelp + "/10";

            etCourseBG1 = FindViewById<EditText>(Resource.Id.etCourseBG1);
            etCourseBG2 = FindViewById<EditText>(Resource.Id.etCourseBG2);
            etCourseBG3 = FindViewById<EditText>(Resource.Id.etCourseBG3);
            etCourseBG4 = FindViewById<EditText>(Resource.Id.etCourseBG4);
            etCourseBG5 = FindViewById<EditText>(Resource.Id.etCourseBG5);

            rdoRole = FindViewById<RadioGroup>(Resource.Id.rdoRole);
            btnStudent = FindViewById<RadioButton>(Resource.Id.btnStudent);
            btnManager = FindViewById<RadioButton>(Resource.Id.btnManager);
            btnAssistantTeacher = FindViewById<RadioButton>(Resource.Id.btnAssistantTeacher);
            btnKitchenHelp = FindViewById<RadioButton>(Resource.Id.btnKitchenHelp);

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            btnDescription = FindViewById<Button>(Resource.Id.btnDescription);
            btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            btnConfirmRegister = FindViewById<Button>(Resource.Id.btnConfirmRegister);
            btnUnregister = FindViewById<Button>(Resource.Id.btnUnregister);
            btnBack = FindViewById<Button>(Resource.Id.btnBack);

            btnDescription.Click += btnDescription_Click;
            btnRegister.Click += btnRegister_Click;
            btnConfirmRegister.Click += btnConfirmRegister_Click;
            btnUnregister.Click += btnUnregister_Click;
            btnBack.Click += btnBack_Click;

            btnDescriptionPressed = false;
            btnRegisterPressed = false;
            int courseHistory = Int32.Parse(pref.GetString("CourseHistory", null));


            DateTime commencementDateTime = DateTime.Parse(commencementDate);
            ////already registered?
            if (DateTime.Now >= commencementDateTime)
            {
                txtCourseRequirements.Text = "The registration period for this course is over.";
                txtCourseRequirements.Visibility = ViewStates.Visible;
                btnRegister.Visibility = ViewStates.Gone;
            }
            else if (courseLength != "10" && courseHistory < 3)
            {
                txtCourseRequirements.Text = "You are unable to register for this course until you have completed at least three ten day courses.";
                btnRegister.Visibility = ViewStates.Gone;
                txtCourseRequirements.Visibility = ViewStates.Visible;
            }
            else if (enrollmentPref.Contains(courseID + "Role"))
            {
                role = enrollmentPref.GetString(courseID + "Role", null);
                txtCourseRequirements.Text = "You are registered as a " +
                    role + " in this course.";
                txtCourseRequirements.Visibility = ViewStates.Visible;
                btnRegister.Visibility = ViewStates.Gone;
                btnUnregister.Visibility = ViewStates.Visible;
            }
            else { setRadioVisibility(); }
        }

        void btnDescription_Click(object sender, EventArgs e)
        {
            btnDescriptionPressed = !btnDescriptionPressed;
            if (btnDescriptionPressed)
            {
                btnDescription.Background = GetDrawable(Resource.Drawable.OpenCourseCalendar);
                txtDescription.Visibility = ViewStates.Visible;
            }
            else
            {
                btnDescription.Background = btnDescription.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton);
                txtDescription.Visibility = ViewStates.Gone;
            }
        }

        void btnRegister_Click(object sender, EventArgs e)
        {
            btnRegisterPressed = !btnRegisterPressed;
            if (btnRegisterPressed)
            {
                btnRegister.Background = GetDrawable(Resource.Drawable.OpenCourseCalendar);
                txtCourseRequirements.Visibility = ViewStates.Visible;
                btnConfirmRegister.Visibility = ViewStates.Visible;
                txtCourseHistory.Visibility = ViewStates.Visible;

                rdoRole.Visibility = ViewStates.Visible;
                
                etCourseBG1.Visibility = ViewStates.Visible;
                etCourseBG2.Visibility = ViewStates.Visible;
                etCourseBG3.Visibility = ViewStates.Visible;
                etCourseBG4.Visibility = ViewStates.Visible;
                etCourseBG5.Visibility = ViewStates.Visible;
            }
            else
            {
                btnRegister.Background = btnDescription.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton);
                btnConfirmRegister.Visibility = ViewStates.Gone;
                txtCourseHistory.Visibility = ViewStates.Gone;

                rdoRole.Visibility = ViewStates.Gone;

                etCourseBG1.Visibility = ViewStates.Gone;
                etCourseBG2.Visibility = ViewStates.Gone;
                etCourseBG3.Visibility = ViewStates.Gone;
                etCourseBG4.Visibility = ViewStates.Gone;
                etCourseBG5.Visibility = ViewStates.Gone;
            }
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            backToCalendar();

        }

        public override void OnBackPressed()
        {
            backToCalendar();
        }

        void backToCalendar()
        {
            var intent = new Intent(this, typeof(calendar));
            StartActivity(intent);
            Finish();
        }

        void btnConfirmRegister_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();

            if(setRole())
            {
                setParameters();


                progressBar.Visibility = ViewStates.Visible;
                client.UploadValuesCompleted += client_UploadValuesCompleted;
                client.UploadValuesAsync(uri, parameters);
                client.Dispose();
            }      
            else
            {
                errorLog.Text = "You must select a role before registering.";
            }      
        }

        bool setRole()
        {
            if(btnStudent.Checked)
            {
                role = "Student";
            }
            else if(btnManager.Checked)
            {
                role = "Manager";
            }
            else if(btnAssistantTeacher.Checked)
            {
                role = "AssistantTeacher";
            }
            else if(btnKitchenHelp.Checked)
            {
                role = "KitchenHelp";
            }
            else { return false; }
            return true;
        }

        void btnUnregister_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();

            parameters = new NameValueCollection();
            studentID = userPref.GetString("CStudentID", null);
            parameters.Add("StudentID", studentID);
            parameters.Add("CourseID", courseID);
            parameters.Add("Role", role);


            progressBar.Visibility = ViewStates.Visible;
            client.UploadValuesCompleted += client_UploadValuesCompleted;
            client.UploadValuesAsync(unregisterUri, parameters);
            client.Dispose();
        }

        void setParameters()
        {
            parameters = new NameValueCollection();
            studentID = userPref.GetString("CStudentID", null);
            parameters.Add("StudentID", studentID);
            parameters.Add("CourseID", courseID);
            parameters.Add("CourseHistoryOne", etCourseBG1.Text);
            parameters.Add("CourseHistoryTwo", etCourseBG2.Text);
            parameters.Add("CourseHistoryThree", etCourseBG3.Text);
            parameters.Add("CourseHistoryFour", etCourseBG4.Text);
            parameters.Add("CourseHistoryFive", etCourseBG5.Text);
            parameters.Add("Role", "Student");
        }

        void setRadioVisibility()
        {
            string gender = enrollmentPref.GetString("Gender", null);

            if (gender == "Male")
            {
                if(Int32.Parse(numMaleStudents) > 25)
                {
                    btnStudent.Visibility = ViewStates.Gone;
                }
                if(maleManager == "Yes")
                {
                    btnManager.Visibility = ViewStates.Gone;
                }
                if(maleTA == "Yes")
                {
                    btnAssistantTeacher.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                if (Int32.Parse(numFemaleStudents) > 25)
                {
                    btnStudent.Visibility = ViewStates.Gone;
                }
                if (femaleManager == "Yes")
                {
                    btnManager.Visibility = ViewStates.Gone;
                }
                if (femaleTA == "Yes")
                {
                    btnAssistantTeacher.Visibility = ViewStates.Gone;
                }
            }
            if(Int32.Parse(numKitchenHelp) > 9)
            {
                btnKitchenHelp.Visibility = ViewStates.Gone;
            }
        }

        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {

                string result = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                result = result.Replace("\r", string.Empty).Replace("\n", string.Empty);

                progressBar.Visibility = ViewStates.Invisible;
                if (result == "error")
                {
                    errorLog.Text = "There was an error processing your request. Please try again later.";
                }
                else
                {
                    updateSQLite(result);
                    backToCalendar();
                }                
            });
        }

        void updateSQLite(string result)
        {
            string query;

            if(enrollmentPref.Contains(courseID + "Role"))//if they already had a role, remove a student from course table
            {
                query = "UPDATE Course SET " + result + " = " + result + " - 1 WHERE CourseID = " + courseID;
            }
            else//if they didn't have a role, add a student to course table
            {
                query = "UPDATE Course SET " + result + " = " + result + " + 1 WHERE CourseID = " + courseID;
            }
            database.updateCourseDetails(courseID, query);
        }
    }
}