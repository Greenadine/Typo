using System;
using System.Data;
using System.Data.SqlClient;
using Typo.Model.Exercise;

namespace Typo.Utils
{
    public class ExerciseResultUtils
    {
        public static ExerciseResultModel? LatestResult { get; set; } // keeps track of the latest result added so we can use it throughout the application

        /// <summary>
        /// Saves the provided exercise result to the database.
        /// </summary>
        /// <param name="exerciseResult">The exercise result to save to the database.</param>
        public static void SaveResult(ExerciseResultModel result)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                // TODO notify failed to reach database
                return;
            }
            LatestResult = result;
            string statement = $"INSERT INTO Statistieken (OefeningID, LeerlingGebruikersnaam, PogingNR, Score, Tijd, HoeveelheidGoed, HoeveelheidFout, ToetsenPerSeconde) VALUES ( @OefeningID, @LeerlingGebruikersnaam, @PogingNR, @Score, @Tijd, @HoeveelheidGoed, @HoeveelheidFout, @ToetsenPerSeconde);";
            SqlCommand sqlCmd = new(statement, connection);

            // Create query parameters
            SqlParameter studentIdParam = new("@LeerlingGebruikersnaam", SqlDbType.Text, 45); ;
            studentIdParam.Value = result.StudentUsername;
            sqlCmd.Parameters.Add(studentIdParam);

            SqlParameter assignmentIdParam = new("@OefeningID", SqlDbType.Int, int.MaxValue);
            assignmentIdParam.Value = result.ExerciseId;
            sqlCmd.Parameters.Add(assignmentIdParam);

            SqlParameter attemptNumberParam = new("@PogingNR", SqlDbType.Int, int.MaxValue);
            attemptNumberParam.Value = result.Attempt;
            sqlCmd.Parameters.Add(attemptNumberParam);

            SqlParameter scoreParam = new("@Score", SqlDbType.Int, int.MaxValue);
            scoreParam.Value = result.Score;
            sqlCmd.Parameters.Add(scoreParam);

            SqlParameter timeParam = new("@Tijd", SqlDbType.Int, int.MaxValue);
            timeParam.Value = result.TimeInSeconds;
            sqlCmd.Parameters.Add(timeParam);

            SqlParameter amountCorrectParam = new("@HoeveelheidGoed", SqlDbType.Int, int.MaxValue);
            amountCorrectParam.Value = result.AmountCorrect;
            sqlCmd.Parameters.Add(amountCorrectParam);

            SqlParameter amountWrongParam = new("@HoeveelheidFout", SqlDbType.Int, int.MaxValue);
            amountWrongParam.Value = result.AmountWrong;
            sqlCmd.Parameters.Add(amountWrongParam);

            SqlParameter keysPerSecondParam = new("@ToetsenPerSeconde", SqlDbType.Decimal);
            keysPerSecondParam.Value = result.KeysPerSecond;
            keysPerSecondParam.Precision = 2;
            keysPerSecondParam.Size = 6;
            sqlCmd.Parameters.Add(keysPerSecondParam);

            // Prepare and execute query
            sqlCmd.Prepare();
            sqlCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the next attempt index for the provided student and exercise.
        /// </summary>
        /// <param name="username">The student username.</param>
        /// <param name="exerciseId">The exercise ID.</param>
        /// <returns>The next attempt index.</returns>
        public static int GetNextAttempt(string username, int exerciseId)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();

            if (connection == null)
            {
                // TODO notify failed to reach database
                return -1;
            }

            string statement = $"SELECT MAX(PogingNR) FROM Statistieken WHERE LeerlingGebruikersnaam = '{username}' AND OefeningID = {exerciseId};";
            SqlCommand command = new(statement, connection);

            object result = command.ExecuteScalar();

            if (result == DBNull.Value)
            {
                return 1;
            }

            return (int)result + 1;
        }
    }
}
