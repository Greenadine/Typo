using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Typo.Model.Exercise;
using Typo.View;
using static System.Data.SqlDbType;

namespace Typo.Utils
{
    public class ExerciseUtils
    {
        public static ExerciseModel? SelectedExercise { get; set; } //for keeping track on the selected exercise through the application

        /// <summary>
        /// Queries all of the exercise data from the database.
        /// </summary>
        /// <returns>An SqlDataReader containing all the exercise data.</returns>
        public static SqlDataReader? GetExercises()
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return null;
            }

            string statement = $"SELECT * FROM Oefening;";
            SqlCommand command = new(statement, connection);
            try
            {
                return command.ExecuteReader();
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return null;
            }
        }

        /// <summary>
        /// Gets the data of all the exercises in the database.
        /// </summary>
        /// <returns>A List containing all the exercieses' data as Dictionaries.</returns>
        public static List<Dictionary<string, object>> GetExercisesData()
        {
            List<Dictionary<string, object>> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return result;
            }

            string statement = $"SELECT * FROM Oefening;";
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
                Dictionary<string, object> exerciseData = new();

                // Get ordinals (indexes) of the columns
                int ordExerciseId = reader.GetOrdinal("OefeningID");
                int ordClassId = reader.GetOrdinal("KlasID");
                int ordName = reader.GetOrdinal("Naam");
                int ordMode = reader.GetOrdinal("Modus");
                int ordDifficulty = reader.GetOrdinal("Moeilijkheid");
                int ordVisibility = reader.GetOrdinal("Zichtbaarheid");
                int ordContent = reader.GetOrdinal("Inhoud");

                // Add data to result dictionary
                exerciseData.Add("OefeningID", reader.GetInt32(ordExerciseId));
                exerciseData.Add("KlasID", reader.GetInt32(ordClassId));
                exerciseData.Add("Naam", reader.GetString(ordName));
                exerciseData.Add("Modus", reader.GetInt32(ordMode));
                exerciseData.Add("Moeilijkheid", reader.GetInt32(ordDifficulty));
                exerciseData.Add("Zichtbaarheid", Convert.ToBoolean(reader.GetByte(ordVisibility)));
                exerciseData.Add("Inhoud", reader.GetString(ordContent));

                result.Add(exerciseData);
            }

            return result;
        }

        /// <summary>
        /// Queries the data of all the exercises from the database that are linked to the provided class.
        /// </summary>
        /// <param name="classId">The ID of the class.</param>
        /// <returns>The SqlReader with all the data of the exercises.</returns>
        public static SqlDataReader? GetExercisesByClass(int classId)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return null;
            }

            string statement = $"SELECT * FROM Oefening WHERE KlasID = {classId};";
            SqlCommand command = new(statement, connection);
            try
            {
                return command.ExecuteReader();
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return null;
            }
        }

        /// <summary>
        /// Checks whether an exercise with the provided ID exists.
        /// </summary>
        /// <param name="id">The exercise ID.</param>
        /// <returns>'true' if an exercise with the provided ID exists, 'false' if no exercises with the provided ID exists.</returns>
        public static bool ExerciseExists(int id)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }

            string statement = $"SELECT COUNT(*) FROM Oefening WHERE OefeningID = {id};";
            SqlCommand command = new(statement, connection);
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
        /// Queries all of an exercise's data.
        /// </summary>
        /// <param name="id">The ID of the exercise/</param>
        /// <returns>An SqlDataReader containing the data of the exercise.</returns>
        public static Dictionary<string, object> GetExerciseById(int id)
        {
            Dictionary<string, object> result = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return result;
            }

            if (!ExerciseExists(id))
            {
                return result;
            }

            string statement = $"SELECT * FROM Oefening WHERE OefeningID = {id};";
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

            if (!reader.Read())
            {
                return result;
            }

            // Get ordinals (indexes) of the columns
            int ordExerciseId = reader.GetOrdinal("OefeningID");
            int ordClassId = reader.GetOrdinal("KlasID");
            int ordName = reader.GetOrdinal("Naam");
            int ordMode = reader.GetOrdinal("Modus");
            int ordDifficulty = reader.GetOrdinal("Moeilijkheid");
            int ordVisibility = reader.GetOrdinal("Zichtbaarheid");
            int ordContent = reader.GetOrdinal("Inhoud");

            // Add data to result dictionary
            result.Add("OefeningID", reader.GetInt32(ordExerciseId));
            result.Add("KlasID", reader.GetInt32(ordClassId));
            result.Add("Naam", reader.GetString(ordName));
            result.Add("Modus", reader.GetInt32(ordMode));
            result.Add("Moeilijkheid", reader.GetInt32(ordDifficulty));
            result.Add("Zichtbaarheid", Convert.ToBoolean(reader.GetByte(ordVisibility)));
            result.Add("Inhoud", reader.GetString(ordContent));

            return result;
        }

        /// <summary>
        /// Inserts a new exercise into the database.
        /// </summary>
        /// <param name="name">The name of the exercise.</param>
        /// <param name="mode">The mode of the exercise.</param>
        /// <param name="difficulty">The difficulty of the exercise.</param>
        /// <param name="classId">The ID of the class to which the exercise is linked.</param>
        /// <param name="visibility">Whether the exercise is visible or not.</param>
        /// <param name="content">The content of the exercise.</param>
        /// <returns>'true' if the exercise was successfuly added to the database, 'false' if the exercises wasn't added to the database.</returns>
        public static bool AddExercise(string name, int classId, int mode, int difficulty, bool visibility, string content)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }

            string statement = $"INSERT INTO Oefening(KlasID, Naam, Modus, Moeilijkheid, Zichtbaarheid, Inhoud) VALUES(@KlasID, @Naam, @Modus, @Moeilijkheid, @Zichtbaarheid, @Inhoud);";
            SqlCommand command = new(statement, connection);

            SqlParameter classIdParam = new("@KlasID", Int);
            classIdParam.Value = classId;
            command.Parameters.Add(classIdParam);

            SqlParameter nameParam = new("@Naam", VarChar, 45);
            nameParam.Value = name;
            command.Parameters.Add(nameParam);

            SqlParameter modeParam = new("@Modus", Int, 1);
            modeParam.Value = mode;
            command.Parameters.Add(modeParam);

            SqlParameter difficultyParam = new("@Moeilijkheid", Int, 1);
            difficultyParam.Value = difficulty;
            command.Parameters.Add(difficultyParam);

            SqlParameter visibilityParam = new("@Zichtbaarheid", Int, 1);
            visibilityParam.Value = visibility;
            command.Parameters.Add(visibilityParam);

            SqlParameter contentParam = new("@Inhoud", VarChar, int.MaxValue);
            contentParam.Value = content;
            command.Parameters.Add(contentParam);

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
