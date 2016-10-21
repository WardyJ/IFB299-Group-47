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
using System.IO;

namespace zenmc
{
    class Database
    {
        public string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Zen.db");

        public string createDatabase()
        {
            try
            {
                SQLiteConnection database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);
                {
                    database.CreateTable<Class>();
                    database.CreateTable<Course>();
                    database.Close();
                    return "Database created";
                }
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        public string insertClass(Class aClass)
        {
            SQLiteConnection conn = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);
            try
            {
                conn.Insert(aClass);
                conn.Close();
                return "Updated";
            }
            catch(SQLiteException ex)
            {
                return ex.Message;
            }
        }

        public void Reset()
        {
            SQLiteConnection conn = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            conn.Execute("DROP TABLE IF EXISTS Class");
            conn.Execute("DROP TABLE IF EXISTS Course");
            conn.Close();
        }

        public string insertCourse(Course course)
        {
            SQLiteConnection conn = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);
            try
            {
                conn.Insert(course);
                conn.Close();
                return "Updated";
            }
            catch(SQLiteException ex)
            {
                return ex.Message;
            }
        }

        public string getCourseDetail(string courseID, string courseDetail)
        {
                var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

                string stmt = "SELECT " + courseDetail + " FROM Course WHERE CourseID = '" + courseID + "'";
                string result = database.ExecuteScalar<string>(stmt);
                database.Close();
                return result;            
        }

        public string getClassDetail(string date, int classNumber, string classDetail)
        {
            var conn = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            string stmt = "SELECT " + classDetail + " FROM Class WHERE substr(DateandTime, 1, 10) = '" + date + "' ORDER BY ClassID LIMIT " + classNumber + "-1, 1";
            string result = conn.ExecuteScalar<string>(stmt);
            conn.Close();
            return result;
        }

        public bool checkDate(string date)
        {
            var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            string stmt = "SELECT DateandTime FROM Class WHERE substr(DateandTime, 1, 10) = '" + date + "'";
            string result = database.ExecuteScalar<string>(stmt);
            database.Close();
            return  result != null;        
        }

        public string getCourseID(string date)
        {
            var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            string stmt = "SELECT DISTINCT CourseID FROM Class WHERE substr(DateandTime, 1,10) = '" + date + "'";
            string result = database.ExecuteScalar<string>(stmt);
            database.Close();
            return result;
        }

        public void updateCourseDetails(string courseID, string query)
        {
            var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);
            database.Execute(query);
            database.Close();
        }
    }
}