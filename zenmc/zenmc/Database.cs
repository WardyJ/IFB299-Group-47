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

        /// <summary>
        /// Sets up a new database connection and creates the tables of the database
        /// </summary>
        /// <returns>string containing details of success or error in creating database</returns>
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

        /// <summary>
        /// Inserts a row of values into the class table
        /// </summary>
        /// <param name="aClass"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes the tables stored in the database
        /// </summary>
        public void Reset()
        {
            SQLiteConnection conn = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            conn.Execute("DROP TABLE IF EXISTS Class");
            conn.Execute("DROP TABLE IF EXISTS Course");
            conn.Close();
        }

        /// <summary>
        /// Inserts new course row into the course table
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves a detail of a course from the course table
        /// </summary>
        /// <param name="courseID">ID of the course being examined</param>
        /// <param name="courseDetail">desired detail of course to be retrieved</param>
        /// <returns>desired detail of a course</returns>
        public string getCourseDetail(string courseID, string courseDetail)
        {
                var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

                string stmt = "SELECT " + courseDetail + " FROM Course WHERE CourseID = '" + courseID + "'";
                string result = database.ExecuteScalar<string>(stmt);
                database.Close();
                return result;            
        }

        /// <summary>
        /// Retrieves a detail of a course from the course table
        /// </summary>
        /// <param name="date">date on which the class occurs</param>
        /// <param name="classNumber">Number of the class on a given date</param>
        /// <param name="classDetail">desired detail of a class to be retrieved</param>
        /// <returns>desired detail of a class</returns>
        public string getClassDetail(string date, int classNumber, string classDetail)
        {
            var conn = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            string stmt = "SELECT " + classDetail + " FROM Class WHERE substr(DateandTime, 1, 10) = '" + date + "' ORDER BY ClassID LIMIT " + classNumber + "-1, 1";
            string result = conn.ExecuteScalar<string>(stmt);
            conn.Close();
            return result;
        }

        /// <summary>
        /// Checks if a date exists in the class database
        /// </summary>
        /// <param name="date">Date being checked</param>
        /// <returns>true if entry with this date exists, false if it doesn't</returns>
        public bool checkDate(string date)
        {
            var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            string stmt = "SELECT DateandTime FROM Class WHERE substr(DateandTime, 1, 10) = '" + date + "'";
            string result = database.ExecuteScalar<string>(stmt);
            database.Close();
            return  result != null;        
        }

        /// <summary>
        /// Get the ID of the course in progress on tha given date
        /// </summary>
        /// <param name="date">Date on which a course is held</param>
        /// <returns>Course ID of the course being held on the date</returns>
        public string getCourseID(string date)
        {
            var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);

            string stmt = "SELECT DISTINCT CourseID FROM Class WHERE substr(DateandTime, 1,10) = '" + date + "'";
            string result = database.ExecuteScalar<string>(stmt);
            database.Close();
            return result;
        }

        /// <summary>
        /// Updates an entry in the course table using given query
        /// </summary>
        /// <param name="courseID">ID of the course to be updated</param>
        /// <param name="query">query to be executed</param>
        public void updateCourseDetails(string courseID, string query)
        {
            var database = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);
            database.Execute(query);
            database.Close();
        }
    }
}