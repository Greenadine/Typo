using System.Collections.Generic;
using System.Data.SqlClient;
using Typo.View;

namespace Typo.Utils
{
    public static class TeacherUtils
    {
        /// <summary>
        /// Queries all of the teacher data from the database.
        /// </summary>
        /// <returns>An SqlDataReader containing all the teacher data.</returns>
        public static List<Dictionary<string, object>> GetTeachersData()
        {
            List<Dictionary<string, object>> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return result;
            }

            string statement = $"SELECT * FROM Docent;";
            SqlCommand command = new(statement, connection);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return result;
            }

            while (reader.Read())
            {
                Dictionary<string, object> teacherData = new();

                // Get ordinals (indexes) of the columns
                int ordUsername = reader.GetOrdinal("Gebruikersnaam");
                int ordName = reader.GetOrdinal("Naam");

                // Add data to result dictionary
                teacherData.Add("Gebruikersnaam", reader.GetString(ordUsername));
                teacherData.Add("Naam", reader.GetString(ordName));
                teacherData.Add("Klassen", GetTeacherClasses((string)teacherData["Gebruikersnaam"]));
                result.Add(teacherData);
            }
            return result;
        }

        /// <summary>
        /// Queries all of a teachers data from the database.
        /// </summary>
        /// <param name="username">The username of the teacher.</param>
        /// <returns>An SqlDataReader containing the teachers data.</returns>
        public static Dictionary<string, object> GetTeacherData(string username)
        {
            Dictionary<string, object> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                // TODO notify failed to reach database
                return result;
            }

            // Get teacher information from Docent table
            string statement = $"SELECT * FROM Docent WHERE Gebruikersnaam = '{username}';";
            SqlCommand command = new(statement, connection);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return result;
            }

            if (!reader.Read()) // If no records were returned
            {
                return result;
            }

            int ordName = reader.GetOrdinal("Naam");

            result.Add("Gebruikersnaam", username);
            result.Add("Naam", reader.GetString(ordName));

            // Get teachers classes from KlasDocentLink table
            result.Add("Klassen", GetTeacherClasses(username));
            return result;
        }

        /// <summary>
        /// Gets the IDs of all the classes a teacher is linked to.
        /// </summary>
        /// <param name="username">The username of the teacher.</param>
        /// <returns>A List containing the IDs of the classes.</returns>
        private static List<int> GetTeacherClasses(string username)
        {
            List<int> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return result;
            }

            string statement = $"SELECT KlasID FROM KlasDocentLink WHERE DocentGebruikersnaam = '{username}'";
            SqlCommand command = new(statement, connection);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return result;
            }

            while (reader.Read())
            {
                int ordClassId = reader.GetOrdinal("KlasID");
                result.Add(reader.GetInt32(ordClassId));
            }
            return result;
        }

        /// <summary>
        /// Checks whether a teacher account with the provided username exists.
        /// </summary>
        /// <param name="username">The teacher username.</param>
        /// <returns>'true' if a teacher account with the provided username exists, 'false' otherwise.</returns>
        public static bool TeacherDataExists(string username)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                return false;
            }

            string statement = $"SELECT COUNT(*) FROM Docent WHERE Gebruikersnaam = @Gebruikersnaam;";
            SqlCommand command = new(statement, connection);

            SqlParameter usernameParam = new("@Gebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            usernameParam.Value = username;
            command.Parameters.Add(usernameParam);

            command.Prepare();
            try
            {
                return ((int)command.ExecuteScalar()) > 0;
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return false;
            }
        }

        /// <summary>
        /// Checks how many teachers exist with the same username, and returns the amount.
        /// </summary>
        /// <param name="username">The username of the teacher.</param>
        /// <returns>The amount of teachers that exist with the same username.</returns>
        public static int TeacherCount(string username)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return -1; // No connection, -1 = error message in the viewmodel.
            }

            string statement = $"SELECT COUNT(Naam) FROM Docent WHERE Naam = '{username}';"; // Count all the teachers with the same name.
            SqlCommand command = new(statement, connection);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
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
        /// Adds a new teacher account to the database.
        /// </summary>
        /// <param name="username">The username of the teacher.</param>
        /// <param name="name">The name of the teacher.</param>
        /// <returns>'true' if the teacher was successfully added to the database, 'false' if adding the teacher to the database failed.</returns>
        public static bool AddTeacher(string username, string name)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }

            string statement = $"INSERT INTO Docent VALUES (@Gebruikersnaam, @Naam);";
            SqlCommand command = new(statement, connection);

            SqlParameter usernameParam = new("@Gebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            usernameParam.Value = username;
            command.Parameters.Add(usernameParam);

            SqlParameter nameParam = new("@Naam", System.Data.SqlDbType.VarChar, 45);
            nameParam.Value = name;
            command.Parameters.Add(nameParam);

            command.Prepare();
            try
            {
                return command.ExecuteNonQuery() >= 1; // If amount of rows affected is 1
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return false;
            }
        }

        /// <summary>
        /// Links the teacher to a class.
        /// </summary>
        /// <param name="username">The username of the teacher.</param>
        /// <param name="classID">The ID of the class.</param>
        /// <returns>'true' if the teacher was successfully linked to the class, 'false' if linking the teacher to the class failed.</returns>
        public static bool LinkTeacherToClass(string username, int classID)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }

            string statement = $"INSERT INTO KlasDocentLink VALUES (@DocentGebruikersnaam, @KlasID);";
            SqlCommand command = new(statement, connection);

            SqlParameter teacherUsernameParam = new("@DocentGebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            teacherUsernameParam.Value = username;
            command.Parameters.Add(teacherUsernameParam);

            SqlParameter classIDParam = new("@KlasID", System.Data.SqlDbType.Int, int.MaxValue);
            classIDParam.Value = classID;
            command.Parameters.Add(classIDParam);

            command.Prepare();
            try
            {
                return command.ExecuteNonQuery() >= 1; // If amount of rows affected is 1
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return false;
            }
        }
    }
}
