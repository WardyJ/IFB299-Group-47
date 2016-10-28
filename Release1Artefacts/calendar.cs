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

namespace meditation_centre
{
    [Activity(Label = "My Calendar")]
    public class calendar : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // set our view from the main layout resource
            SetContentView(Resource.Layout.calendar);
            var calendarView = FindViewById<CalendarView>(Resource.Id.calendarView);
            var txtDisplay = FindViewById<TextView>(Resource.Id.txtDisplay);
            var eventView = FindViewById<TextView>(Resource.Id.eventView);

            eventView.Text = "Events for Today";
            txtDisplay.Text = "Date: ";
            calendarView.DateChange += (s, e) =>
            {
                int day = e.DayOfMonth;
                int month = e.Month;
                int year = e.Year;
                txtDisplay.Text = "Date: " + day + "/" + month + "/" + year;
                eventView.Text = "Display some event when the date changes.";
            };

            //SetContentView(Resource.Layout.calendar);
        }
    }
}