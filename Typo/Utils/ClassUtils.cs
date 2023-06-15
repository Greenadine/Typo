using System.Collections.Generic;
using System.Data.SqlClient;

namespace Typo.Utils
{
    public class ClassUtils
    {
        /// <summary>
        /// Gets all of the data from all classes in the database.
        /// </summary>
        /// <returns>A List containing the data of all classes in the database as a Dictionary.</returns>
        public static List<Dictionary<string, object>> GetClassesData()
        {
            List<Dictionary<string, object>> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return result;
            }

            string statement = $"SELECT * FROM Klas;";
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

            while (reader.Read())
            {
                Dictionary<string, object> classData = new();

                // Get ordinals (indexes) of the columns
                int ordClassId = reader.GetOrdinal("KlasID");
                int ordName = reader.GetOrdinal("KlasNaam");

                // Add data to result dictionary
                classData.Add("KlasID", reader.GetInt32(ordClassId));
                classData.Add("Naam", reader.GetString(ordName));

                result.Add(classData);
            }
            return result;
        }

        /// <summary>
        /// Retrieves the ID of a specific class from the database.
        /// </summary>
        /// <param name="Classname">The name of the class.</param>
        /// <returns>The ID of the class with the provided username.</returns>
        public static int GetClassID(string Classname)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return -1;
            }

            string statement = $"SELECT KlasID FROM Klas WHERE Klasnaam = @Klasnaam;";
            SqlCommand command = new(statement, connection);

            SqlParameter ClassnameParam = new("@Klasnaam", System.Data.SqlDbType.VarChar, 45);
            ClassnameParam.Value = Classname;
            command.Parameters.Add(ClassnameParam);

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
                return reader.GetInt32(0);
            }

            return -1;
        }

        /// <summary>
        /// Checks if a class already exist in the database with the given name.
        /// </summary>
        /// <param name="Classname">The name of the class.</param>
        /// <returns>'true' if a class with the provided name already exists, 'false' if no class exists with the provided name.</returns>
        public static int CheckClassName(string Classname)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return -1;
            }

            string statement = $"SELECT COUNT(Klasnaam) FROM Klas WHERE Klasnaam = @Klasnaam;";
            SqlCommand command = new(statement, connection);

            SqlParameter ClassnameParam = new("@Klasnaam", System.Data.SqlDbType.VarChar, 45);
            ClassnameParam.Value = Classname;
            command.Parameters.Add(ClassnameParam);

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
                return reader.GetInt32(0);
            }
            return -1;
        }

        /// <summary>
        /// Adds a new Class to the database.
        /// Adds a new class to the database with the provided name.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <returns>'true' if the class was successfully added to the database, 'false' if the class was not added to the database.</returns>
        public static bool AddClass(string Classname)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }

            string statement = $"INSERT INTO Klas VALUES (@Klasnaam);";
            SqlCommand command = new(statement, connection);

            SqlParameter ClassnameParam = new("@Klasnaam", System.Data.SqlDbType.VarChar, 45);
            ClassnameParam.Value = Classname;
            command.Parameters.Add(ClassnameParam);

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
        /// <summary>
        /// Checks if the teacher is already linked to the class, so there are no primary key errors in the database.
        /// </summary>
        /// <param name="Username">The username of the teacher.</param>
        /// <param name="ClassID">The ID of the class.</param>
        /// <returns>'true' if the teacher is already linked to the class, 'false' if the teacher is not already linked to the class.</returns>
        public static int CheckTeacherClassLink(string Username, int ClassID)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return -1;
            }

            string statement = $"SELECT COUNT(DocentGebruikersnaam) FROM KlasDocentLink WHERE DocentGebruikersnaam = @DocentGebruikersnaam AND KlasID = @KlasID;";
            SqlCommand command = new(statement, connection);

            SqlParameter UsernameParam = new("@DocentGebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            UsernameParam.Value = Username;
            command.Parameters.Add(UsernameParam);

            SqlParameter ClassIDParam = new("@KlasID", System.Data.SqlDbType.Int, int.MaxValue);
            ClassIDParam.Value = ClassID;
            command.Parameters.Add(ClassIDParam);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch
            {
                return -1;
            }

            while (reader.Read())
            {
                return reader.GetInt32(0);
            }

            return -1;
        }
    }
}
