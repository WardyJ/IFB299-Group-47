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
        private const int daysInWeek = 7;
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

            database = new Database();

            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            userPref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            enrollmentPref = Application.Context.GetSharedPreferences("EnrollmentInfo", FileCreationMode.Private);
            
            setLayoutResources();            
            
            string studentID = userPref.GetString("CStudentID", null);
            parameters.Add("StudentID", studentID);

            infoParameters.Add("StudentID", studentID);

            //If first time coming to calendar with this student id, get student information
            if (studentID != "Owner" && studentID != "Reception" 
                && userPref.GetString("NewInfo", null) == "true")
            {
                getStudentInfo();
                getEnrollmentData();        
                organizeEnrollmentData();
            }
                        
            initializeCalendar();
        }

        /// <summary>
        /// Finds all of the controls in the layout and assigns them to variables so they
        /// can be referenced later and subscribes buttons to events
        /// </summary>
        void setLayoutResources()
        {
            btnForward = FindViewById<Button>(Resource.Id.btnForward);
            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnClassInfo1 = FindViewById<Button>(Resource.Id.btnClassInfo1);
            btnClassInfo2 = FindViewById<Button>(Resource.Id.btnClassInfo2);
            btnClassInfo3 = FindViewById<Button>(Resource.Id.btnClassInfo3);
            btnMealInfo = FindViewById<Button>(Resource.Id.btnMealInfo);
            btnCourseInfo = FindViewById<Button>(Resource.Id.btnCourseInfo);

            btnBack.Click += btnBack_Click;
            btnForward.Click += btnForward_Click;
            btnClassInfo1.Click += btnClassInfo1_Click;
            btnClassInfo2.Click += btnClassInfo2_Click;
            btnClassInfo3.Click += btnClassInfo3_Click;
            btnMealInfo.Click += btnMealInfo_Click;
            btnCourseInfo.Click += btnCourseInfo_Click;

            row1 = FindViewById<LinearLayout>(Resource.Id.row1);
            row2 = FindViewById<LinearLayout>(Resource.Id.row2);
            row3 = FindViewById<LinearLayout>(Resource.Id.row3);
            row4 = FindViewById<LinearLayout>(Resource.Id.row4);
            row5 = FindViewById<LinearLayout>(Resource.Id.row5);
            row6 = FindViewById<LinearLayout>(Resource.Id.row6);
            row7 = FindViewById<LinearLayout>(Resource.Id.row7);

            txtDisplay = FindViewById<TextView>(Resource.Id.txtDisplay);
            txtMonth = FindViewById<TextView>(Resource.Id.txtMonth);

            btnParams = FindViewById(Resource.Id.btn1).LayoutParameters;//buttons created for the calendar will copy the parameters of this button
        }

        /// <summary>
        /// Accesses student information stored on the server related to the student ID
        /// being used to find the student's gender and type (old or new)
        /// </summary>
        void getStudentInfo()
        {
            studentClient = new WebClient();
            string json = Encoding.UTF8.GetString(studentClient.UploadValues(infoUri, infoParameters));
            studentClient.Dispose();
            studentInfo = JsonConvert.DeserializeObject<List<Student>>(json);
            
            editor = userPref.Edit();
            editor.PutString("NewInfo", "false");
            editor.PutString("Gender", studentInfo[0].Gender);
            editor.PutString("Type", studentInfo[0].StudentType);
            editor.Apply();
        }

        /// <summary>
        /// Accesses course enrollment information stored on the server and stores
        /// the course IDs belonging to courses the student is enrolled in
        /// </summary>
        void getEnrollmentData()
        {            
            client = new WebClient();
            string json = Encoding.UTF8.GetString(client.UploadValues(uri, parameters));
            client.Dispose();

            editor = pref.Edit();
            editor.PutString("EnrollmentDetails", json);
            editor.Apply();
        }        

        /// <summary>
        /// Calls methods to find the date information needed to populate the calendar's first
        /// set of rows, set the rows up and select the current date
        /// </summary>
        void initializeCalendar()
        {
            setDateValues();
            populateRows();           

            selectDate(firstDayOfWeek + initialDay);
        }

        /// <summary>
        /// Takes the enrollment information downloaded from the server and stores it in 
        /// shared preferences so it can be easily accessed from anywhere in the app
        /// </summary>
        void organizeEnrollmentData()
        {
            string json = pref.GetString("EnrollmentDetails", string.Empty);
            editor = enrollmentPref.Edit();

            enrollmentInfo = JsonConvert.DeserializeObject<List<EnrollmentDetails>>(json);
            foreach (EnrollmentDetails enrollment in enrollmentInfo)
            {
                string courseID = enrollment.CourseID;
                editor.PutString(courseID + "Role", enrollment.Role);
            }
            editor.Apply();
        }

        /// <summary>
        /// Assigns to variables information on either the last date selected, or if none was 
        /// then the current date. Sets the displayed month/year on screen to the selected date.
        /// </summary>
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

            getMonthValues();
        }

        /// <summary>
        /// Calls methods to populate the 7 rows of the calendar
        /// </summary>
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

        /// <summary>
        /// Sets the first row of the calendar to display the names of the days of the week
        /// </summary>
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
        
        /// <summary>
        /// sets up the dates of the calendar with clickable buttons displaying each date
        /// of the selected month
        /// </summary>
        /// <param name="row">The ID of the row's layout in the page layout</param>
        /// <param name="rowNum">The number of the row currently being populated</param>
        void populateRow(LinearLayout row, int rowNum)
        {
            int day = (rowNum - 2) * daysInWeek;
            for (int i = 1; i <= daysInWeek; i++)
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

        /// <summary>
        /// Assigns a background to a button based on the student's ability to register as a
        /// student (referred to as status) for the course on the date represented by 
        /// that button and whether or not the button is currently selected.
        /// </summary>
        /// <param name="status">represents by the student's ability to register for
        /// the course. May be "Closed", "Full", "Open" or "Registered"</param>
        /// <param name="btn">A button of the calendar</param>
        /// <param name="selection">Whether or not the button is currently being selected</param>
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

        /// <summary>
        /// Event that occurs when the back button is clicked on the calendar. Sets the
        /// calendar date back one month and calls a method to repopulate the calendar to
        /// represent that month.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event that occurs when the forward button is clicked on the calendar. Sets the
        /// calendar date forward one month and calls a method to repopulate the calendar to
        /// represent that month.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Removes the stored data about which date is currently selected
        /// </summary>
        void forgetCurrentSelection()
        {
            if (pref.Contains("DateSelectedID"))
            {
                editor = pref.Edit();
                editor.Remove("DateSelectedID");
                editor.Apply();
            }
        }

        /// <summary>
        /// Assigns dates to variables based on the current month selected and sets the displayed
        /// month/year text to represent the current date
        /// </summary>
        void getMonthValues()
        {
            firstOfMonth = new DateTime(year, month, 1);

            txtMonth.Text = new DateTime(year, month, 1)
    .ToString("MMMM", CultureInfo.InvariantCulture) + " " + year;


            daysInMonth = DateTime.DaysInMonth(year, month);
            firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            lastDayOfMonth = DateTime.DaysInMonth(year, month);
        }

        /// <summary>
        /// Calls methods to repopulate the rows of the calendar
        /// </summary>
        void repopulateRows()
        {
            repopulateRow(row2, 2);
            repopulateRow(row3, 3);
            repopulateRow(row4, 4);
            repopulateRow(row5, 5);
            repopulateRow(row6, 6);
            repopulateRow(row7, 7);
        }

        /// <summary>
        /// Resets the buttons on the row to have different dates and events based on the
        /// selected month
        /// </summary>
        /// <param name="row">The layout resource for the row in the displayed layout provided</param>
        /// <param name="rowNum">The number of the given row of the calendar</param>
        void repopulateRow(LinearLayout row, int rowNum)
        {
            int day = (rowNum - 2) * daysInWeek;
            for (int i = 1; i <= daysInWeek; i++)
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

        /// <summary>
        /// Event that occurs when a button of the calendar is clicked. Removes the event
        /// information buttons from the display and call method to select the date clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDay_Click(object sender, EventArgs e)
        {            
            removeEventButtons();
            Button btn = (Button)sender;
            selectDate(btn.Id); 
        }

        /// <summary>
        /// Finds and stores information about the date represented by the button being selected
        /// </summary>
        /// <param name="buttonId">Id of the button being selected</param>
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
            txtDisplay.Text = reformattedDate + "      ";

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

        /// <summary>
        /// Removes information stored about the selected date and calls method to set the
        /// background to be a deselected style.
        /// </summary>
        void deselectDate()
        {
            string previousId = pref.GetString("DateSelectedID", null);
            string previousDate = pref.GetString("DateSelected", null);
            string courseID = database.getCourseID(previousDate);
            Button btnPrevious = FindViewById<Button>(Int32.Parse(previousId));
            string status = getStatus(previousDate);
            setStatus(status, btnPrevious, false);
        }

        /// <summary>
        /// Examines a date and determines whether the student can enroll into a course
        /// being held on the given date.
        /// </summary>
        /// <param name="date">String representing a date to be examined</param>
        /// <returns>String representing the students ability to register to a course on
        /// a certain date</returns>
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
            string gender = userPref.GetString("Gender", null);
            if(gender == "Male" && numMales >= maxStudents || gender == "Female" && numFemales >= maxStudents)
            {
                return "Full";
            }
            return "Open";
        }

        /// <summary>
        /// Makes the course, class and meal information buttons visible on the display
        /// </summary>
        void addEventButtons()
        {
            btnCourseInfo.Visibility = ViewStates.Visible;
            btnClassInfo1.Visibility = ViewStates.Visible;
            btnClassInfo2.Visibility = ViewStates.Visible;
            btnClassInfo3.Visibility = ViewStates.Visible;
            btnMealInfo.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Removes the course, class and meal information buttons from view.
        /// </summary>
        void removeEventButtons()
        {
            btnCourseInfo.Visibility = ViewStates.Invisible;
            btnClassInfo1.Visibility = ViewStates.Invisible;
            btnClassInfo2.Visibility = ViewStates.Invisible;
            btnClassInfo3.Visibility = ViewStates.Invisible;
            btnMealInfo.Visibility = ViewStates.Invisible;
        }

        /// <summary>
        /// Calls method to display class info related to the button when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClassInfo1_Click(object sender, EventArgs e)
        {
            displayClassInfo(1);
        }

        /// <summary>
        /// Calls method to display class info related to the button when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClassInfo2_Click(object sender, EventArgs e)
        {
            displayClassInfo(2);
        }

        /// <summary>
        /// Calls method to display class info related to the button when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClassInfo3_Click(object sender, EventArgs e)
        {
            displayClassInfo(3);
        }

        /// <summary>
        /// Retrieves data related to a class and bundles it into a dialog fragment to be
        /// displayed.
        /// </summary>
        /// <param name="classNumber">Number of the class being selected.</param>
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
            string date = pref.GetString("DateSelected", null);
            string time1 = database.getMealDetails(date, 1, "DateandTime");
            string time2 = database.getMealDetails(date, 2, "DateandTime");
            string time3 = database.getMealDetails(date, 3, "DateandTime");
            string details1 = database.getMealDetails(date, 1, "Details");
            string details2 = database.getMealDetails(date, 2, "Details");
            string details3 = database.getMealDetails(date, 3, "Details");
/*
            Bundle classBundle = new Bundle();
            mealBundle.PutString("Time1", time1);
            mealBundle.PutString("Time2", time2);
            mealBundle.PutString("Time3", time3);
            mealBundle.PutString("Details1", details1);
            mealBundle.PutString("Details1", details1);
            mealBundle.PutString("Details1", details1);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            mealDialog dialog = new mealDialog();
            dialog.Arguments = mealBundle;
            //Show class dialog fragment
            dialog.Show(transaction, "ClassDialog");*/
        }

        /// <summary>
        /// Event that occurs when the course information button is clicked. Gets course ID
        /// of the selected date and sends the user to the related course information page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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