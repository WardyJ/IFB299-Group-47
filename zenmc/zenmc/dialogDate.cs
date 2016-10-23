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
using Android.Util;

namespace zenmc
{
    public class DialogDate : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        //tag used to identify the fragment.
        public static readonly string tag = "X:" + typeof(DialogDate).Name.ToUpper();
        
        Action<DateTime> dateSelectedHandler = delegate { };

        /// <summary>
        /// Method that is called when a new date picker dialog fragment is being opened
        /// </summary>
        /// <param name="onDateSelected"></param>
        /// <returns></returns>
        public static DialogDate NewInstance(Action<DateTime> onDateSelected)
        {
            DialogDate frag = new DialogDate();
            frag.dateSelectedHandler = onDateSelected;
            return frag;
        }

        /// <summary>
        /// Method for setting up a new datepickerdialog fragment. Minimum and maximum allowed
        /// values for date are set.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DatePickerDialog dialog = new DatePickerDialog(Activity, 3, this, 1980, 0, 1);
            var origin = new DateTime(1970, 1, 1);//date from which other dates are measured
            dialog.DatePicker.MaxDate = (long)(DateTime.Now.Date.AddYears(-18) - origin).TotalMilliseconds;
            dialog.DatePicker.MinDate = (long)(DateTime.Now.Date.AddYears(-116) - origin).TotalMilliseconds;
            return dialog;
        }

        /// <summary>
        /// Gets the DateTime of a date based on the date values selected in the datepicker
        /// </summary>
        /// <param name="view">The datepicker being used</param>
        /// <param name="year">year value selected in the picker</param>
        /// <param name="monthOfYear">month value selected in the picker</param>
        /// <param name="dayOfMonth">day value selected in the picker</param>
        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(tag, selectedDate.ToLongDateString());
            dateSelectedHandler(selectedDate);
        }
    }
}