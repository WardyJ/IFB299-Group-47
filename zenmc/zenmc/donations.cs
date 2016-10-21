
using Android.App;
using Android.OS;

namespace zenmc
{
    [Activity(Label = "Donation Info", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class donations : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.donations);


        }
    }
}