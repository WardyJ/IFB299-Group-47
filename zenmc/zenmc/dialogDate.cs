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
    public class DialogDate : DialogFragment,
                                  DatePickerDialog.IOnDateSetListener
    {
        //tag used to identify the gragment.
        public static readonly string tag = "X:" + typeof(DialogDate).Name.ToUpper();
        
        Action<DateTime> dateSelectedHandler = delegate { };

        public static DialogDate NewInstance(Action<DateTime> onDateSelected)
        {
            DialogDate frag = new DialogDate();
            frag.dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = new DatePickerDialog(Activity, 3, this, 1980, 0, 1);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(tag, selectedDate.ToLongDateString());
            dateSelectedHandler(selectedDate);
        }
    }
}