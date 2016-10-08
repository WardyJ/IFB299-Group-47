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
    class Student
    {
        [JsonProperty("StudentID")]
        public int StudentID { get; set; }
        [JsonProperty("FullName")]
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string StudentType { get; set; }
        public string Gender { get; set; }
        public string PasswordHash { get; set; }
    }
}