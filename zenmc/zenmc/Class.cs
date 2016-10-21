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
using Newtonsoft.Json;
using SQLite;

namespace zenmc
{

    public class Class
    {
        [PrimaryKey]
        public string ClassID { get; set; }

        public string CourseID { get; set; }
        public string ClassType { get; set; }
        public string Length { get; set; }
        public string Description { get; set; }
        public string DateandTime { get; set; }
        public string RecordingID { get; set; }

        public override string ToString()
        {
            return string.Format("[Person: CourseID={0}, CourseNameID={1}, ClassID={2}, ClassType={3}, Length={4}, Description={5}, DateandTime={6}, RecordingID={7}");
        }
    }
}