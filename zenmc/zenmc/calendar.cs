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
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Android.Preferences;

namespace zenmc
{
    [Activity(Label = "My Calendar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class calendar : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/enrollmentinfo.php");
        private Uri infoUri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/getinfo.php");

        WebClient client;
        WebClient studentClient;
        private LinearLayout row1, row2, row3, row4, row5, row6, row7;
        private TextView txtMonth, txtDisplay;
        ViewGroup.LayoutParams btnParams;
        private Button btnBack, btnForward, btnClassInfo1, btnClassInfo2, 
            btnClassInfo3, btnCourseInfo, btnMealInfo;
        private int firstDayOfWeek, lastDayOfMonth, daysInMonth, initialDay, month, year;
        private DateTime firstOfMonth, dateSelected;
        private NameValueCollection parameters = new NameValueCollection();
        private NameValueCollection infoParameters = new NameValueCollection();
        private ISharedPreferences pref, enrollmentPref, userPref;
        private ISharedPreferencesEditor editor;
        private Database database;

        private const int maxStudents = 26;

        public List<Student> studentInfo;
        public List<EnrollmentDetails> enrollmentInfo;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.calendar);
            txtDisplay = FindViewById<TextView>(Resource.Id.txtDisplay);

            database = new Database();

            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            userPref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);

            enrollmentPref = Application.Context.GetSharedPreferences("EnrollmentInfo", FileCreationMode.Private);

            btnForward = FindViewById<Button>(Resource.Id.btnForward);
            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnClassInfo1 = FindViewById<Button>(Resource.Id.btnClassInfo1);
            btnClassInfo2 = FindViewById<Button>(Resource.Id.btnClassInfo2);
            btnClassInfo3 = FindViewById<Button>(Resource.Id.btnClassInfo3);
            btnMealInfo = FindViewById<Button>(Resource.Id.btnMealInfo);
            btnCourseInfo = FindViewById<Button>(Resource.Id.btnCourseInfo);

            row1 = FindViewById<LinearLayout>(Resource.Id.row1);
            row2 = FindViewById<LinearLayout>(Resource.Id.row2);
            row3 = FindViewById<LinearLayout>(Resource.Id.row3);
            row4 = FindViewById<LinearLayout>(Resource.Id.row4);
            row5 = FindViewById<LinearLayout>(Resource.Id.row5);
            row6 = FindViewById<LinearLayout>(Resource.Id.row6);
            row7 = FindViewById<LinearLayout>(Resource.Id.row7);
            txtMonth = FindViewById<TextView>(Resource.Id.txtMonth);
            

            btnParams = FindViewById(Resource.Id.btn1).LayoutParameters;
            
            string studentID = userPref.GetString("CStudentID", null);
            parameters.Add("StudentID", studentID);

            infoParameters.Add("StudentID", studentID);
            
            //if(!pref.Contains("CalendarSaved"))
            //{
                getStudentInfo();

            client = new WebClient();
                string json = Encoding.UTF8.GetString(client.UploadValues(uri, parameters));
                client.Dispose();

                editor = pref.Edit();

                editor.PutString("EnrollmentDetails", json);
                editor.Apply();
                editor = pref.Edit();
                organizeEnrollmentData();
            //}
            
            initializeCalendar();
        }

        void getStudentInfo()
        {
            studentClient = new WebClient();
            string json = Encoding.UTF8.GetString(studentClient.UploadValues(infoUri, infoParameters));
            studentClient.Dispose();
            studentInfo = JsonConvert.DeserializeObject<List<Student>>(json);

            editor = enrollmentPref.Edit();
            editor.Clear();
            editor.Apply();
            editor.PutString("Gender", studentInfo[0].Gender);
            editor.PutString("Type", studentInfo[0].StudentType);
            editor.Apply();
        }

        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string json = Encoding.UTF8.GetString(e.Result);

                editor = pref.Edit();

                editor.PutString("EnrollmentDetails", json);
                editor.Apply();
                editor = pref.Edit();
                organizeEnrollmentData();

                initializeCalendar();
            });
        }

        void initializeCalendar()
        {
            setDateValues();

            populateRows();
            
            btnBack.Click += btnBack_Click;
            btnForward.Click += btnForward_Click;
            btnClassInfo1.Click += btnClassInfo1_Click;
            btnClassInfo2.Click += btnClassInfo2_Click;
            btnClassInfo3.Click += btnClassInfo3_Click;
            btnMealInfo.Click += btnMealInfo_Click;
            btnCourseInfo.Click += btnCourseInfo_Click;

            string classDate = new DateTime(year, month, firstDayOfWeek + DateTime.Now.Day).ToString("yyyy,MM,dd");

            editor.PutString("CalendarSaved", "true");
            editor.Apply();

            selectDate(firstDayOfWeek + initialDay);
        }

        
        void organizeEnrollmentData()
        {
            string json = pref.GetString("EnrollmentDetails", string.Empty);
            editor = enrollmentPref.Edit();

            enrollmentInfo = JsonConvert.DeserializeObject<List<EnrollmentDetails>>(json);
            int count = 0;
            foreach (EnrollmentDetails enrollment in enrollmentInfo)
            {
                string courseID = enrollment.CourseID;
                editor.PutString(courseID + "Role", enrollment.Role);
                editor.Apply();
                count += 1;
            }
            editor = pref.Edit();
            editor.PutString("CourseHistory", count.ToString());
            editor.Apply();
        }

        void setDateValues()
        {
            DateTime date;

            if (pref.Contains("DateSelected"))
            {
                date = DateTime.Parse(pref.GetString("DateSelected", null));
                initialDay = date.Day;
                month = date.Month;
                year = date.Year;
            }
            else
            {
                date = DateTime.Now;
                initialDay = date.Day;
                month = date.Month;
                year = date.Year;
            }
            

            firstOfMonth = new DateTime(year, month, 1);

            txtMonth.Text = new DateTime(year, month, 1)
    .ToString("MMMM", CultureInfo.InvariantCulture) + " " + year;


            daysInMonth = DateTime.DaysInMonth(year, month);
            firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            lastDayOfMonth = DateTime.DaysInMonth(year, month);
        }

        void populateRows()
        {
            firstRow();
            populateRow(row2,2);
            populateRow(row3,3);
            populateRow(row4,4);
            populateRow(row5,5);
            populateRow(row6,6);
            populateRow(row7,7);
        }

        void firstRow()
        {
            string[] days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                Button btnDay = new Button(Application.Context);
                btnDay.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton);
                btnDay.Text = days[i];
                btnDay.TextSize = 12;
                btnDay.LayoutParameters = btnParams;
                row1.AddView(btnDay);
            }            
        }
        
        void populateRow(LinearLayout row, int rowNum)
        {
            int day = (rowNum - 2) * 7;
            for (int i = 1; i < 8; i++)
            {
                day += 1;
                Button btnDay = new Button(Application.Context);
                if (day >= firstDayOfWeek + 1 && day <= firstDayOfWeek + lastDayOfMonth)
                {
                    int dayOfMonth = day - firstDayOfWeek;
                    btnDay.Text = dayOfMonth.ToString();
                    btnDay.Click += btnDay_Click;

                    DateTime dayDate = new DateTime(year, month, dayOfMonth);
                    

                    if (database.checkDate(dayDate.ToString("yyyy-MM-dd")))
                    {
                        string status = getStatus(dayDate.ToString("yyyy-MM-dd"));
                        setStatus(status, btnDay, false);
                    }
                    else { btnDay.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton); }
                }
                else { btnDay.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton); }
                btnDay.LayoutParameters = btnParams;
                btnDay.Id = day;
                
                row.AddView(btnDay);
                if (rowNum == 7)
                {
                    btnDay.Visibility = ViewStates.Visible;

                    if (day - i >= daysInMonth + firstDayOfWeek)
                    {
                        btnDay.Visibility = ViewStates.Invisible;
                    }
                }
            }
        }

        void setStatus(string status, Button btn, bool selection)
        {
            if (status == "Closed")
            {
                if(selection)
                {
                    btn.Background = GetDrawable(Resource.Drawable.ClosedCourseSelected);
                }
                else { btn.Background = GetDrawable(Resource.Drawable.ClosedCourseCalendar); }
                
            }
            else if (status == "Registered")
            {
                if(selection)
                {
                    btn.Background = GetDrawable(Resource.Drawable.RegisteredCourseSelected);
                }
                else { btn.Background = GetDrawable(Resource.Drawable.RegisteredCourseCalendar); }
            }
            else if (status == "Full")
            {
                if (selection)
                {
                    btn.Background = GetDrawable(Resource.Drawable.FullCourseSelected);
                }                
                else{ btn.Background = GetDrawable(Resource.Drawable.FullCourseCalendar); }
            }
            else if (status == "Open")
            {
                if (selection)
                {
                    btn.Background = GetDrawable(Resource.Drawable.OpenCourseSelected);
                }
                else { btn.Background = GetDrawable(Resource.Drawable.OpenCourseCalendar); }
            }            
            else
            {
                if(selection)
                {
                    btn.Background = GetDrawable(Resource.Drawable.DefaultCalendarSelected);
                }
                else { btn.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton); }
            }
        } 

        void btnBack_Click(object sender, EventArgs e)
        {
            forgetCurrentSelection();
            if(month > 1)
            {
                month -= 1;                
            }
            else
            {
                month = 12;
                year -= 1;
            }
            getMonthValues();
            repopulateRows();
        }

        void btnForward_Click(object sender, EventArgs e)
        {
            forgetCurrentSelection();
            if (month < 12)
            {
                month += 1;                
            }
            else
            {
                year += 1;
                month = 1;
            }
            getMonthValues();
            repopulateRows();
        }

        void forgetCurrentSelection()
        {
            if (pref.Contains("DateSelectedID"))
            {
                editor = pref.Edit();
                editor.Remove("DateSelectedID");
                editor.Apply();
            }
        }

        void getMonthValues()
        {
            firstOfMonth = new DateTime(year, month, 1);

            txtMonth.Text = new DateTime(year, month, 1)
    .ToString("MMMM", CultureInfo.InvariantCulture) + " " + year;


            daysInMonth = DateTime.DaysInMonth(year, month);
            firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            lastDayOfMonth = DateTime.DaysInMonth(year, month);
        }

        void repopulateRows()
        {
            repopulateRow(row2, 2);
            repopulateRow(row3, 3);
            repopulateRow(row4, 4);
            repopulateRow(row5, 5);
            repopulateRow(row6, 6);
            repopulateRow(row7, 7);
        }

        void repopulateRow(LinearLayout row, int rowNum)
        {
            int day = (rowNum - 2) * 7;
            for (int i = 1; i < 8; i++)
            {
                day += 1;
                Button btnDay = FindViewById<Button>(day);
                btnDay.Click -= btnDay_Click;
                if (day >= firstDayOfWeek + 1 && day <= firstDayOfWeek + lastDayOfMonth)
                {
                    int dayOfMonth = day - firstDayOfWeek;
                    btnDay.Text = dayOfMonth.ToString();
                    btnDay.Click += btnDay_Click;

                    DateTime dayDate = new DateTime(year, month, dayOfMonth);
                    
                    string status = getStatus(dayDate.ToString("yyyy-MM-dd"));
                    setStatus(status, btnDay, false);                    
                }
                else
                {
                    btnDay.Text = "";
                    btnDay.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton);
                }
                
                if (rowNum == 7)
                {
                    btnDay.Visibility = ViewStates.Visible;

                    if (day - i >= daysInMonth + firstDayOfWeek)
                    {
                        btnDay.Visibility = ViewStates.Invisible;
                    }
                }
            }
        }
        

        void btnDay_Click(object sender, EventArgs e)
        {
            
            removeEventButtons();
            Button btn = (Button)sender;
            selectDate(btn.Id); 
        }

        void selectDate(int buttonId)
        {
            string status;
            string courseID; 
            

            Button btnCurrent = FindViewById<Button>(buttonId);

            if (pref.Contains("DateSelectedID"))
            {
                deselectDate();
            }
            

            dateSelected = new DateTime(year, month, Int32.Parse(btnCurrent.Text));
            string defaultDate = dateSelected.ToString("yyyy-MM-dd");

            string reformattedDate = dateSelected.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            txtDisplay.Text = "Date: " + reformattedDate;

            courseID = database.getCourseID(defaultDate);
            status = getStatus(defaultDate);
            
            setStatus(status, btnCurrent, true);
            
            editor = pref.Edit();
            
            editor.PutString("DateSelectedID", buttonId.ToString());
            editor.PutString("DateSelected", defaultDate);
            editor.Apply();

            if (database.checkDate(defaultDate))
            {
                addEventButtons();
            }
        }

        void deselectDate()
        {
            string previousId = pref.GetString("DateSelectedID", null);
            string previousDate = pref.GetString("DateSelected", null);
            string courseID = database.getCourseID(previousDate);
            Button btnPrevious = FindViewById<Button>(Int32.Parse(previousId));
            string status = getStatus(previousDate);
            setStatus(status, btnPrevious, false);
        }

        string getStatus(string date)
        {
            if(!database.checkDate(date))
            {
                return "Default";
            }

            string courseID = database.getCourseID(date);
            bool registered = enrollmentPref.Contains(courseID + "Role");
            if(registered)
            {
                return "Registered";
            }
            DateTime commencementDate = DateTime.Parse(database.getCourseDetail(courseID, "CommencementDate"));
            if(DateTime.Now >= commencementDate)
            {
                return "Closed";
            }
            int numFemales = Int32.Parse(database.getCourseDetail(courseID, "FemaleStudents"));
            int numMales = Int32.Parse(database.getCourseDetail(courseID, "MaleStudents"));
            string gender = enrollmentPref.GetString("Gender", null);
            if(gender == "Male" && numMales >= maxStudents || gender == "Female" && numFemales >= maxStudents)
            {
                return "Full";
            }
            return "Open";
        }

        void addEventButtons()
        {
            btnCourseInfo.Visibility = ViewStates.Visible;
            btnClassInfo1.Visibility = ViewStates.Visible;
            btnClassInfo2.Visibility = ViewStates.Visible;
            btnClassInfo3.Visibility = ViewStates.Visible;
            btnMealInfo.Visibility = ViewStates.Visible;
        }

        void removeEventButtons()
        {
            btnCourseInfo.Visibility = ViewStates.Invisible;
            btnClassInfo1.Visibility = ViewStates.Invisible;
            btnClassInfo2.Visibility = ViewStates.Invisible;
            btnClassInfo3.Visibility = ViewStates.Invisible;
            btnMealInfo.Visibility = ViewStates.Invisible;
        }

        void btnClassInfo1_Click(object sender, EventArgs e)
        {
            displayClassInfo(1);
        }

        void btnClassInfo2_Click(object sender, EventArgs e)
        {
            displayClassInfo(2);
        }

        void btnClassInfo3_Click(object sender, EventArgs e)
        {
            displayClassInfo(3);
        }

        void displayClassInfo(int classNumber)
        {
            string date = pref.GetString("DateSelected", null);
            string type = database.getClassDetail(date, classNumber, "ClassType");
            string description = database.getClassDetail(date, classNumber, "Description");
            string dateAndTime = database.getClassDetail(date, classNumber, "DateandTime");
            string length = database.getClassDetail(date, classNumber, "Length");

            string time = dateAndTime.Substring(11, 5);

            Bundle classBundle = new Bundle();
            classBundle.PutString("Type", type);
            classBundle.PutString("Description", description);
            classBundle.PutString("Time", time);
            classBundle.PutString("Length", length);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            classDialog dialog = new classDialog();
            dialog.Arguments = classBundle;
            //Show class dialog fragment
            dialog.Show(transaction, "ClassDialog");
        }

        void btnMealInfo_Click(object sender, EventArgs e)
        {

        }

        void btnCourseInfo_Click(object sender, EventArgs e)
        {            
            string dateSelected = pref.GetString("DateSelected", null);
            string courseID = database.getCourseID(dateSelected);
            forgetCurrentSelection();//Instead go back to current date when users navigate back to calendar again

            var intent = new Intent(this, typeof(coursePage));
            intent.PutExtra("CourseID", courseID);
            StartActivity(intent);
            Finish();
        }
    }
}