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

namespace Menu_Navigation
{
    class MyListViewAdapter : BaseAdapter<Navigation>
    {
        private List<Navigation> mItems;
        private Context mContext;

        public MyListViewAdapter(Context context, List<Navigation> items)
        {
            mItems = items;
            mContext = context;
        }
        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Navigation this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.listview_row, null, false);
            }

            TextView TheButton = row.FindViewById<TextView>(Resource.Id.TheButton);
            TheButton.Text = mItems[position].TheButton;

            return row;

        }
    }
}