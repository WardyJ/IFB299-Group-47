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

namespace meditation_centre
{
    [Activity(Label = "Report A Bug")]
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
            errormessageEdit = FindViewById<EditText>(meditation_centre.Resource.Id.errormessageEdit);
            sendButton = FindViewById<Button>(meditation_centre.Resource.Id.sendButton);
            nameEdit = FindViewById<EditText>(meditation_centre.Resource.Id.nameEdit);
            sendButton.Click += SendButton_Click;
            
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string errorMessage = errormessageEdit.Text;
            string name = nameEdit.Text;
            SmsManager.Default.SendTextMessage("0402069278", null, (name + "\n \n"+ errorMessage), null, null);
        }
    }
}