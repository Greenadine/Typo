using System;
using System.Collections.Generic;
using Typo.View;

namespace Typo.Utils
{
    public static class UserUtils
    {
        public static UserType UserType;
        public static string Username = string.Empty;

        /// <summary>
        /// Logs in with the user with the provided username.
        /// </summary>
        /// 
        /// <param name="username">The username of the account to login with.</param>
        /// 
        /// <returns>
        /// 'true' if the login was successful, 'false' if no account with the provided username was found;
        /// </returns>
        public static bool Login(string username)
        {
            if (StudentUtils.StudentDataExists(username))
            {
                Username = username;
                UserType = UserType.Student;
            }
            else if (TeacherUtils.TeacherDataExists(username))
            {
                Username = username;
                UserType = UserType.Teacher;
            }
            else if (AdminUtils.AdminDataExists(username))
            {
                Username = username;
                UserType = UserType.Admin;
            }
            else
            {
                return false; // No account found with username
            }
            return true;
        }

        /// <summary>
        /// Logs out of the current logged-in account.
        /// </summary>
        public static void Logout()
        {
            UserType = UserType.None;
            Username = string.Empty;
        }

        /// <summary>
        /// Gets the data of the current student.
        /// </summary>
        /// <returns>A Dictionary containing the data of the current student.</returns>
        /// <exception cref="Exception">Thrown if the method cannot be called in the current state.</exception>
        public static Dictionary<string, object> GetCurrentStudent()
        {
            if (UserType == UserType.None)
            {
                Logout();
                App.CurrentWindow = new LoginView();
                return new();
            }
            else if (UserType != UserType.Student)
            {
                throw new Exception("Cannot call this method: current user is not a student.");
            }
            return StudentUtils.GetStudentData(Username);
        }

        /// <summary>
        /// Gets the name of the current logged-in student.
        /// </summary>
        /// <returns>The student's name.</returns>
        public static string GetCurrentStudentName()
        {
            Dictionary<string, object> studentData = GetCurrentStudent();
            if (!studentData.ContainsKey("Naam"))
            {
                Logout();
                App.CurrentWindow = new LoginView();
                return string.Empty;
            }
            return (string)GetCurrentStudent()["Naam"];
        }

        /// <summary>
        /// Gets the data of the current teacher.
        /// </summary>
        /// <returns>A Dictionary containing the data of the current teacher.</returns>
        /// <exception cref="Exception">Thrown if the method cannot be called in the current state.</exception>
        public static Dictionary<string, object> GetCurrentTeacher()
        {
            if (UserType == UserType.None)
            {
                Logout();
                App.CurrentWindow = new LoginView();
                return new();
            }
            else if (UserType != UserType.Teacher)
            {
                throw new Exception("Cannot call this method: current user is not a teacher.");
            }
            return TeacherUtils.GetTeacherData(Username);
        }

        /// <summary>
        /// Gets the name of the currently logged-in teacher.
        /// </summary>
        /// <returns>The teacher's name.</returns>
        public static string GetCurrentTeacherName()
        {
            Dictionary<string, object> teacherData = GetCurrentTeacher();
            if (!teacherData.ContainsKey("Naam"))
            {
                Logout();
                App.CurrentWindow = new LoginView();
                return string.Empty;
            }
            return (string)teacherData["Naam"];
        }

        /// <summary>
        /// Gets the data of the current admin.
        /// </summary>
        /// <returns>A Dictionary containing the data of the current admin.</returns>
        /// <exception cref="Exception">Thrown if the method cannot be called in the current state.</exception>
        public static Dictionary<string, object> GetCurrentAdmin()
        {
            if (UserType == UserType.None)
            {
                Logout();
                App.CurrentWindow = new LoginView();
                return new();
            }
            else if (UserType != UserType.Admin)
            {
                throw new Exception("Cannot call this method: current user is not a admin.");
            }
            return AdminUtils.GetAdminData(Username);
        }

        /// <summary>
        /// Gets the name of the currently logged-in admin.
        /// </summary>
        /// <returns>The admin's name.</returns>
        public static string GetCurrentAdminName()
        {
            Dictionary<string, object> adminData = GetCurrentAdmin();
            if (!adminData.ContainsKey("Naam"))
            {
                Logout();
                App.CurrentWindow = new LoginView();
                return string.Empty;
            }
            return (string)adminData["Naam"];
        }
    }
}