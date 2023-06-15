using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Typo.Utils
{
    public class StudentUtils
    {
        /// <summary>
        /// Queries all of the student data from the database.
        /// </summary>
        /// <returns>An SqlDataReader containing all the student data.</returns>
        public static SqlDataReader? GetStudentsData()
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                // TODO notify failed to reach database
                return null;
            }

            string statement = $"SELECT * FROM Leerling;";
            SqlCommand command = new(statement, connection);
            try
            {
                return command.ExecuteReader();
            }
            catch (SqlException)
            {
                return null;
            }
        }

        /// <summary>
        /// Queries all of the student data from students that are not yet assigned to a class.
        /// </summary>
        /// <returns>An SqlDataReader containing all the student data.</returns>
        public static List<Dictionary<string, object>> GetClasslessStudentsData()
        {
            List<Dictionary<string, object>> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                // TODO notify failed to reach database
                return result;
            }

            string statement = $"SELECT * FROM Leerling WHERE KlasID IS NULL;";
            SqlCommand command = new(statement, connection);

            SqlDataReader? reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                return result;
            }

            while (reader.Read())
            {
                Dictionary<string, object> studentData = new();

                int ordUsername = reader.GetOrdinal("Gebruikersnaam");
                int ordName = reader.GetOrdinal("Naam");
                int ordClassId = 0;

                studentData.Add("Gebruikersnaam", reader.GetString(ordUsername));
                studentData.Add("Naam", reader.GetString(ordName));
                studentData.Add("KlasID", ordClassId);

                result.Add(studentData);
            }

            return result;
        }

        /// <summary>
        /// Queries all of a students data from the database.
        /// </summary>
        /// <param name="username">The student accounts username.</param>
        /// <returns>An SqlDataReader containing the students data.</returns>
        public static Dictionary<string, object> GetStudentData(string username)
        {
            Dictionary<string, object> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                // TODO notify failed to reach database
                return result;
            }

            string statement = $"SELECT * FROM Leerling WHERE Gebruikersnaam = '{username}'";
            SqlCommand command = new(statement, connection);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                return result;
            }

            if (!reader.Read())
            {
                return result; // No records were returned
            }

            int ordName = reader.GetOrdinal("Naam");
            int ordClassId = reader.GetOrdinal("KlasID");

            result.Add("Gebruikersnaam", username);
            result.Add("Naam", reader.GetString(ordName));
            if (!reader.IsDBNull(ordClassId))
            {
                result.Add("KlasID", reader.GetInt32(ordClassId));
            }
            return result;
        }

        /// <summary>
        /// Updates the data of a student.
        /// </summary>
        /// <param name="username">The student's username.</param>
        /// <param name="name">The student's (new) name.</param>
        /// <param name="classID">The student's (new) class ID.</param>
        /// <returns>'true' if the update was successful, 'false' otherwise.</returns>
        public static bool UpdateStudentData(string username, string name, int? classID)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }

            string statement = $"UPDATE Leerling SET Naam = @Naam, KlasID = @KlasID WHERE Gebruikersnaam = @Gebruikersnaam;";
            SqlCommand command = new(statement, connection);

            SqlParameter NameParam = new("@Naam", SqlDbType.VarChar, 45);
            NameParam.Value = name;
            command.Parameters.Add(NameParam);

            SqlParameter classIdParam = new("@KlasID", SqlDbType.Int, int.MaxValue);
            classIdParam.Value = classID;
            command.Parameters.Add(classIdParam);

            SqlParameter UsernameParam = new("@Gebruikersnaam", SqlDbType.VarChar, 45);
            UsernameParam.Value = username;
            command.Parameters.Add(UsernameParam);

            command.Prepare();
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        /// <summary>
        /// Queries the name of a student from the database.
        /// </summary>
        /// <param name="username">The student account's username.</param>
        /// <returns>The name of the student from the database.</returns>
        public static string GetStudentName(string username)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return string.Empty;
            }

            string statement = $"SELECT Naam FROM Leerling WHERE Gebruikersnaam = @Gebruikersnaam;";
            SqlCommand sqlCmd = new(statement, connection);

            SqlParameter usernameParam = new("@Gebruikersnaam", SqlDbType.VarChar, 45);
            usernameParam.Value = username;
            sqlCmd.Parameters.Add(usernameParam);

            sqlCmd.Prepare();
            object result = sqlCmd.ExecuteScalar();

            return result is not null ? (string)result : string.Empty;
        }

        /// <summary>
        /// Checks whether a student account with the provided username exists.
        /// </summary>
        /// <param name="username">The accounts username.</param>
        /// <returns>'true' if a student account with the provided username exists, 'false' otherwise.</returns>
        public static bool StudentDataExists(string username)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                // TODO notify failed to reach database
                return false;
            }

            string statement = $"SELECT COUNT(*) FROM Leerling WHERE Gebruikersnaam = '{username}';"; ;
            SqlCommand command = new(statement, connection);

            try
            {
                return ((int)command.ExecuteScalar()) > 0;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the amount of studends with the same name and return and int
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static int StudentCount(string username)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                // TODO notify failed to reach database
                return -1;
            }

            string statement = $"SELECT COUNT(Naam) FROM Leerling WHERE Naam = '{username}';"; ;
            SqlCommand command = new(statement, connection);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                return -1;
            }

            while (reader.Read())
            {
                int result = reader.GetInt32(0);
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Adds a new student to the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddStudent(string username, string name)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }

            string statement = $"INSERT INTO Leerling VALUES (@Gebruikersnaam, @Naam, @KlasID);";
            SqlCommand command = new(statement, connection);

            SqlParameter usernameParam = new("@Gebruikersnaam", SqlDbType.VarChar, 45);
            usernameParam.Value = username;
            command.Parameters.Add(usernameParam);

            SqlParameter nameParam = new("@Naam", SqlDbType.VarChar, 45);
            nameParam.Value = name;
            command.Parameters.Add(nameParam);

            SqlParameter classIdParam = new("@klasID", SqlDbType.Int, int.MaxValue);
            classIdParam.Value = DBNull.Value;
            command.Parameters.Add(classIdParam);

            command.Prepare();
            try
            {
                return command.ExecuteNonQuery() >= 1; // If amount of rows affected is 1
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}
