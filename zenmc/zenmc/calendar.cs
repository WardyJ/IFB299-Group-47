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

namespace zenmc
{
    [Activity(Label = "My Calendar")]
    public class calendar : Activity
    {
        private Uri uri = new Uri("http://ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com/datetest.php");

        WebClient client = new WebClient();
        public List<Classes> classInfo;
        private LinearLayout row1, row2, row3, row4, row5, row6, row7;
        private TextView txtMonth, txtDisplay;
        ViewGroup.LayoutParams btnParams;
        private Button btnBack, btnForward, btnClassInfo1, btnClassInfo2, 
            btnClassInfo3, btnCourseInfo, btnMealInfo;
        private int firstDayOfWeek, lastDayOfMonth, daysInMonth, month, year;
        private DateTime firstOfMonth, dateSelected;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // set our view from the main layout resource
            SetContentView(Resource.Layout.calendar);
            txtDisplay = FindViewById<TextView>(Resource.Id.txtDisplay);

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

            client.DownloadDataCompleted += client_DownloadDataCompleted;
            client.DownloadDataAsync(uri);
            
        }

        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string json = Encoding.UTF8.GetString(e.Result);

                ISharedPreferences pref = Application.Context.GetSharedPreferences("CalendarInfo", FileCreationMode.Private);

                ISharedPreferencesEditor editor = pref.Edit();

                editor.PutString("ClassDetails", json);
                editor.Apply();
                editor = pref.Edit();
                organizeDateData();

                
                editor.PutString("DateSelectedID", (firstDayOfWeek + DateTime.Now.Day).ToString());
                editor.Apply();

                editor = pref.Edit();

                if (pref.Contains(new DateTime(year, month, firstDayOfWeek + DateTime.Now.Day).ToString("yyyy,MM,dd")))
                {
                    editor.PutString("previousDrawable", "open");
                    addEventButtons();
                }
                else
                {
                    editor.PutString("previousDrawable", "default");
                }

                editor.Apply();

                selectDate(firstDayOfWeek + DateTime.Now.Day);

                if (pref.Contains(new DateTime(year, month, firstDayOfWeek + DateTime.Now.Day).ToString("yyyy,MM,dd")))
                {
                    addEventButtons();
                }
            });
        }

        void organizeDateData()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("CalendarInfo", FileCreationMode.Private);
            string json = pref.GetString("ClassDetails", string.Empty);
            ISharedPreferencesEditor editor = pref.Edit();

            classInfo = JsonConvert.DeserializeObject<List<Classes>>(json);
            
            foreach(Classes session in classInfo)
            {
                string date = session.DateandTime;
                date = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                editor.PutString(date, date);
                editor.PutString(date + "CourseID", session.CourseID);
                editor.PutString(date + "Status", "open");
                editor.PutString(date + "CourseName", session.CourseName);
                editor.PutString(date + "ClassID", session.ClassID);
                editor.PutString(date + "ClassType", session.ClassType);
                editor.PutString(date + "Length", session.Length);
                editor.PutString(date + "Description", session.Description);
                editor.PutString(date + "DateandTime", session.DateandTime);
                editor.PutString(date + "RecordingID", session.RecordingID);
                editor.Apply();
            }
            year = 2016;
            month = DateTime.Now.Month;

            firstOfMonth = new DateTime(year, month, 1);

            txtMonth.Text = new DateTime(year, month, 1)
    .ToString("MMMM", CultureInfo.InvariantCulture);


            daysInMonth = DateTime.DaysInMonth(year, month);
            firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            lastDayOfMonth = DateTime.DaysInMonth(year, month);


            populateRows();

            btnBack.Click += btnBack_Click;
            btnForward.Click += btnForward_Click;
            btnClassInfo1.Click += btnClassInfo1_Click;
            btnClassInfo2.Click += btnClassInfo2_Click;
            btnClassInfo3.Click += btnClassInfo3_Click;
            btnMealInfo.Click += btnMealInfo_Click;
            btnCourseInfo.Click += btnCourseInfo_Click;
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
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("CalendarInfo", FileCreationMode.Private);

                    if (pref.Contains(dayDate.ToString("yyyy-MM-dd")))
                    {
                        btnDay.Background = GetDrawable(Resource.Drawable.OpenCourseCalendar);
                        ISharedPreferencesEditor editor = pref.Edit();
                        btnDay.Click += btnEvent_Click;
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

        void btnBack_Click(object sender, EventArgs e)
        {
            forgetCurrentSelection();
            if(month > 1)
            {
                month -= 1;
                getMonthValues();
                repopulateRows();
            }
        }

        void btnForward_Click(object sender, EventArgs e)
        {
            forgetCurrentSelection();
            if(month < 12)
            {
                month += 1;
                getMonthValues();
               repopulateRows();
            }
        }

        void forgetCurrentSelection()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("CalendarInfo", FileCreationMode.Private);
            if (pref.Contains("DateSelectedID"))
            {
                ISharedPreferencesEditor editor = pref.Edit();
                editor.Remove("DateSelectedID");
                editor.Apply();
            }
        }

        void getMonthValues()
        {
            firstOfMonth = new DateTime(year, month, 1);

            txtMonth.Text = new DateTime(year, month, 1)
    .ToString("MMMM", CultureInfo.InvariantCulture);


            daysInMonth = DateTime.DaysInMonth(year, month);
            firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            lastDayOfMonth = DateTime.DaysInMonth(2016, month);
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
                btnDay.Click -= btnEvent_Click;
                if (day >= firstDayOfWeek + 1 && day <= firstDayOfWeek + lastDayOfMonth)
                {
                    int dayOfMonth = day - firstDayOfWeek;
                    btnDay.Text = dayOfMonth.ToString();
                    btnDay.Click += btnDay_Click;

                    DateTime dayDate = new DateTime(year, month, dayOfMonth);
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("CalendarInfo", FileCreationMode.Private);
                    
                    if (pref.Contains(dayDate.ToString("yyyy-MM-dd")))
                    {
                        btnDay.Background = GetDrawable(Resource.Drawable.OpenCourseCalendar);
                        btnDay.Click -= btnDay_Click;
                        btnDay.Click += btnEvent_Click;
                    }
                    else { btnDay.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton); }
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
            ISharedPreferences pref = Application.Context.GetSharedPreferences("CalendarInfo", FileCreationMode.Private);
            string previousId = pref.GetString("DateSelectedID", null);
            if(previousId != null)
            {
                Button btnPrevious = FindViewById<Button>(Int32.Parse(previousId));
                string previousDrawable = pref.GetString("previousDrawable", string.Empty);
                if(previousDrawable == "default")
                {
                    btnPrevious.Background = GetDrawable(Resource.Drawable.DefaultCalendarButton);
                }
                else if(previousDrawable == "open")
                {
                    btnPrevious.Background = GetDrawable(Resource.Drawable.OpenCourseCalendar);
                }
                
            }

            Button btn = FindViewById<Button>(buttonId);
            
            

            dateSelected = new DateTime(year, month, Int32.Parse(btn.Text));
            string defaultDate = dateSelected.ToString("yyyy-MM-dd");
            string stringDate = dateSelected.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            txtDisplay.Text = "Date: " + stringDate;

            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("selectedDate", defaultDate);
            if (pref.GetString(defaultDate + "Status", null) == "open")
            {
                editor.PutString("previousDrawable", "open");
                btn.Background = GetDrawable(Resource.Drawable.OpenCourseSelected);
            }
            else
            {
                editor.PutString("previousDrawable", "default");
                btn.Background = GetDrawable(Resource.Drawable.DefaultCalendarSelected);
            }
            
            editor.PutString("DateSelectedID", buttonId.ToString());
            editor.PutString("DateSelected", dateSelected.ToString());
            editor.Apply();
        }

        void btnEvent_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            selectDate(btn.Id);
            addEventButtons();
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

        }

        void btnClassInfo2_Click(object sender, EventArgs e)
        {

        }

        void btnClassInfo3_Click(object sender, EventArgs e)
        {

        }

        void btnMealInfo_Click(object sender, EventArgs e)
        {

        }

        void btnCourseInfo_Click(object sender, EventArgs e)
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("CalendarInfo", FileCreationMode.Private);
            string dateSelected = pref.GetString("dateSelected", null);
            string courseID = pref.GetString(dateSelected + "CourseID", null);

            var intent = new Intent(this, typeof(course));
            intent.PutExtra("CourseID", courseID);
            StartActivity(intent);
        }
    }
}