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
using Android.Telephony;

namespace zenmc
{
    [Activity(Label = "Report A Bug", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class smsTest : Activity
    {
        EditText errormessageEdit;
        EditText nameEdit;
        Button sendButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.smsTest);

            //Edit phone number
            errormessageEdit = FindViewById<EditText>(Resource.Id.errormessageEdit);
            sendButton = FindViewById<Button>(Resource.Id.sendButton);
            nameEdit = FindViewById<EditText>(Resource.Id.nameEdit);
            sendButton.Click += SendButton_Click;

        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string errorMessage = errormessageEdit.Text;
            string name = nameEdit.Text;
            SmsManager.Default.SendTextMessage("0402069278", null, (name + "\n \n" + errorMessage), null, null);
        }
    }
}