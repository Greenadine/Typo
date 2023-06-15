using System.Collections.Generic;
using System.Data.SqlClient;

namespace Typo.Utils
{
    public static class StatisticsUtils
    {

        /// <summary>
        /// Gets the data of all the exercise results of a user from the database.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A List containing the data of all the exercise results as a Dictionary.</returns>
        public static List<Dictionary<string, object>> GetStatisticsListData(string username)
        {
            List<Dictionary<string, object>> list = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return list;
            }

            string statement = $"SELECT s.*,o.Naam FROM Statistieken AS s join Oefening O ON s.OefeningID = O.OefeningID WHERE LeerlingGebruikersnaam = @LeerlingGebruikersnaam";
            SqlCommand sqlCmd = new(statement, connection);

            SqlParameter studentIdParam = new("@LeerlingGebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            studentIdParam.Value = username;
            sqlCmd.Parameters.Add(studentIdParam);

            sqlCmd.Prepare();

            SqlDataReader reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                Dictionary<string, object> data = new();
                int ordOefeningID = reader.GetOrdinal("OefeningID");
                int ordOefeningNaam = reader.GetOrdinal("Naam");
                int ordPogingNR = reader.GetOrdinal("PogingNR");
                int ordScore = reader.GetOrdinal("Score");
                int ordAfrondDatum = reader.GetOrdinal("AfrondDatum");
                int ordTijd = reader.GetOrdinal("Tijd");
                int ordHoeveeleheidGoed = reader.GetOrdinal("HoeveelheidGoed");
                int ordHoeveelheidFout = reader.GetOrdinal("HoeveelheidFout");
                int ordToetsenPerSeconde = reader.GetOrdinal("ToetsenPerSeconde");

                data["OefeningID"] = reader.GetInt32(ordOefeningID);
                data["OefeningNaam"] = reader.GetString(ordOefeningNaam);
                data["PogingNR"] = reader.GetInt32(ordPogingNR);
                data["Score"] = reader.GetInt32(ordScore);
                data["AfrondDatum"] = reader.GetDateTime(ordAfrondDatum);
                data["Tijd"] = reader.GetInt32(ordTijd);
                data["HoeveelheidGoed"] = reader.GetInt32(ordHoeveeleheidGoed);
                data["HoeveelheidFout"] = reader.GetInt32(ordHoeveelheidFout);
                data["ToetsenPerSeconde"] = reader.GetDecimal(ordToetsenPerSeconde);
                list.Add(data);
            }
            return list;
        }
    }
}
