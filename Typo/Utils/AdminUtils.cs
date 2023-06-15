using System.Collections.Generic;
using System.Data.SqlClient;

namespace Typo.Utils
{
    public class AdminUtils
    {
        /// <summary>
        /// Gets the data of the 
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The data of the admin.</returns>
        public static Dictionary<string, object> GetAdminData(string username)
        {
            Dictionary<string, object> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                return result;
            }

            string statement = $"SELECT * FROM Applicatiebeheerder WHERE Gebruikersnaam = @Gebruikersnaam;";
            SqlCommand command = new(statement, connection);

            SqlParameter usernameParam = new("@Gebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            usernameParam.Value = username;
            command.Parameters.Add(usernameParam);

            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException)
            {
                return result;
            }

            if (!reader.Read()) // If no records were returned
            {
                return result;
            }

            int ordName = reader.GetOrdinal("Naam");

            result.Add("Gebruikersnaam", username);
            result.Add("Naam", reader.GetString(ordName));
            return result;
        }

        /// <summary>
        /// Checks whether an admin account with the provide username exists.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>'true' if an admin with the provided username exists, 'false' if no admin with the provided username exists.</returns>
        public static bool AdminDataExists(string username)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                return false;
            }

            string statement = $"SELECT COUNT(*) FROM Applicatiebeheerder WHERE Gebruikersnaam = @Gebruikersnaam;";
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
                return false;
            }
        }
    }
}
