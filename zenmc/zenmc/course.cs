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
using SQLite;

namespace zenmc
{
    public class Course
    {
        [PrimaryKey]
        public string CourseID { get; set; }

        public string CourseName { get; set; }
        public string Length { get; set; }
        public string CommencementDate { get; set; }
        public string Description { get; set; }
        public string MaleStudents { get; set; }
        public string FemaleStudents { get; set; }
        public string MaleManagers { get; set; }
        public string FemaleManagers { get; set; }
        public string MaleTAs { get; set; }
        public string FemaleTAs { get; set; }
        public string KitchenHelp { get; set; }
    }
}