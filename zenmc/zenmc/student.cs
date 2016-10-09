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
    public class Student
    {
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string StudentType { get; set; }
        public string Gender { get; set; }
        public string PasswordHash { get; set; }
        public string MedicalConditions { get; set; }
        public string PrescribedMedication { get; set; }
        public string ContactName { get; set; }
        public string Relationship { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string ZipOrPostcode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}