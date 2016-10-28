using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;

namespace Menu_Navigation
{
	[Activity (Label = "Menu_Navgiation", MainLauncher = true, Icon = "@drawable/icon", Theme="@style/MyTheme")]
	public class MainActivity : ActionBarActivity
	{
		private SupportToolbar mToolbar;
		private MyActionBarDrawerToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		private ListView mLeftDrawer;
		private ArrayAdapter mLeftAdapter;
        private List<string> mLeftDataSet;
        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
	
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.myListView);

			mLeftDrawer.Tag = 0;

			SetSupportActionBar(mToolbar);
		
			//mLeftDataSet = new List<Navigation>();

            mLeftDataSet = new List<string>();
            mLeftDataSet.Add("Home");
            mLeftDataSet.Add("My Calendar");
            mLeftDataSet.Add("My Profile");
            mLeftDataSet.Add("Contact");
            mLeftDataSet.Add("Dontations");
            mLeftDataSet.Add("Send Bug Report");
            mLeftDataSet.Add("Log Out");


            mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);

            //MyListViewAdapter adapter = new MyListViewAdapter(this, mLeftDataSet);

            //mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, 30);

            mLeftDrawer.Adapter = mLeftAdapter;
            mLeftDrawer.ItemClick += mLeftDrawer_ItemClick;

            mDrawerToggle = new MyActionBarDrawerToggle(
				this,							//Host Activity
				mDrawerLayout,					//DrawerLayout
				Resource.String.openDrawer,		//Opened Message
				Resource.String.closeDrawer		//Closed Message
			);

			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayShowTitleEnabled(true);
			mDrawerToggle.SyncState();

		}

        void mLeftDrawer_ItemClick(ListView l, View v, int position, long id)
        {
            //base.OnListItemClick(l, v, position, id);
            var value = mLeftDataSet[position];
            if (value == "My Calendar")
            {
                Intent intent = new Intent(this, typeof(calendar));
                StartActivity(intent);
            }
            if (value == "My Profile")
            {
                Intent intent = new Intent(this, typeof(profile));
                StartActivity(intent);
            }
            if (value == "Contacts")
            {
                Intent intent = new Intent(this, typeof(contacts));
                StartActivity(intent);
            }
            if (value == "Donations")
            {
                Intent intent = new Intent(this, typeof(dontation));
                StartActivity(intent);
            }
        }
			
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.action_menu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}
	}
}


