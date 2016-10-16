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

namespace zenmc
{

    public class Classes
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string ClassID { get; set; }
        public string ClassType { get; set; }
        public string Length { get; set; }
        public string Description { get; set; }
        public string DateandTime { get; set; }
        public string RecordingID { get; set; }
    }
}