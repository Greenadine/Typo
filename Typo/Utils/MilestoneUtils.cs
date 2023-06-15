using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Typo.Model.Exercise;
using Typo.Model.Milestone;
using Typo.ViewModel;

namespace Typo.Utils
{
    public static class MilestoneUtils
    {
        /// <summary>
        /// Gets the Milestone data from the DB
        /// </summary>
        /// <param name="username">The username of the current student</param>
        /// <returns>The data used to construct a Milestone</returns>
        public static List<Dictionary<string, Object>> GetMilestones(string username)
        {
            List<Dictionary<string, Object>> list = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return list;
            }

            string statement = $"select m.*,MP.HuidigeProgressie,MP.AfrondDatum from Milestone as m join MilestoneProgressie MP on m.MilestoneID = MP.MilestoneID where LeerlingGebruikersnaam = @LeerlingGebruikersnaam";
            SqlCommand sqlCmd = new SqlCommand(statement, connection);

            SqlParameter studentIdParam = new SqlParameter("@LeerlingGebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            studentIdParam.Value = username;
            sqlCmd.Parameters.Add(studentIdParam);

            sqlCmd.Prepare();

            SqlDataReader reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {

                Dictionary<string, Object> data = new();
                int ordMilstoneID = reader.GetOrdinal("MilestoneID");
                int ordProgression = reader.GetOrdinal("HuidigeProgressie");
                int ordCompletionDate = reader.GetOrdinal("AfrondDatum");
                int ordName = reader.GetOrdinal("Naam");
                int ordDescription = reader.GetOrdinal("Beschrijving");
                int ordType = reader.GetOrdinal("Type");
                int ordThreshold = reader.GetOrdinal("Drempelwaarde");
                int ordScoreThreshold = reader.GetOrdinal("ScoreDrempelwaarde");
                int ordImagePath = reader.GetOrdinal("AfbeeldingPad");


                data["MilestoneID"] = reader.GetInt32(ordMilstoneID);
                data["Progression"] = reader.GetInt32(ordProgression);
                if (!reader.IsDBNull(ordCompletionDate))
                { //CompletionDate can be null
                    data["CompletionDate"] = reader.GetDateTime(ordCompletionDate);
                }
                data["Name"] = reader.GetString(ordName);
                data["Description"] = reader.GetString(ordDescription);
                data["Type"] = reader.GetInt32(ordType);
                data["Threshold"] = reader.GetInt32(ordThreshold);
                if (!reader.IsDBNull(ordScoreThreshold))
                {
                    data["ScoreThreshold"] = reader.GetInt32(ordScoreThreshold);
                }
                data["ImagePath"] = reader.GetString(ordImagePath);
                list.Add(data);
            }
            return list;
        }

        public static List<Dictionary<string, Object>> GetIncompleteMilestones(string username)
        {
            List<Dictionary<string, Object>> list = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return list;
            }

            string statement = $"select m.*,MP.HuidigeProgressie,MP.AfrondDatum from Milestone as m join MilestoneProgressie MP on m.MilestoneID = MP.MilestoneID where LeerlingGebruikersnaam = @LeerlingGebruikersnaam and AfrondDatum is null";
            SqlCommand sqlCmd = new SqlCommand(statement, connection);

            SqlParameter studentIdParam = new SqlParameter("@LeerlingGebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            studentIdParam.Value = username;
            sqlCmd.Parameters.Add(studentIdParam);

            sqlCmd.Prepare();

            SqlDataReader reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {

                Dictionary<string, Object> data = new();
                int ordMilstoneID = reader.GetOrdinal("MilestoneID");
                int ordProgression = reader.GetOrdinal("HuidigeProgressie");
                int ordCompletionDate = reader.GetOrdinal("AfrondDatum");
                int ordName = reader.GetOrdinal("Naam");
                int ordDescription = reader.GetOrdinal("Beschrijving");
                int ordType = reader.GetOrdinal("Type");
                int ordThreshold = reader.GetOrdinal("Drempelwaarde");
                int ordScoreThreshold = reader.GetOrdinal("ScoreDrempelwaarde");
                int ordImagePath = reader.GetOrdinal("AfbeeldingPad");
                data["MilestoneID"] = reader.GetInt32(ordMilstoneID);
                data["Progression"] = reader.GetInt32(ordProgression);
                if (!reader.IsDBNull(ordCompletionDate))
                { //CompletionDate can be null
                    data["CompletionDate"] = reader.GetDateTime(ordCompletionDate);
                }
                data["Name"] = reader.GetString(ordName);
                data["Description"] = reader.GetString(ordDescription);
                data["Type"] = reader.GetInt32(ordType);
                data["Threshold"] = reader.GetInt32(ordThreshold);
                if (!reader.IsDBNull(ordScoreThreshold))
                {
                    data["ScoreThreshold"] = reader.GetInt32(ordScoreThreshold);
                }
                data["ImagePath"] = reader.GetString(ordImagePath);
                list.Add(data);
            }
            return list;
        }

        /// <summary>
        /// Function used to update the progression of a milestone in the DB
        /// </summary>
        /// <param name="milestoneID">The ID of the milestone</param>
        /// <param name="username">The username of the current student</param>
        /// <param name="progression">The value of progression of the milestone</param>
        /// <param name="completionDate">The date the milestone is completed. This may be null</param>
        /// <returns>A boolean representing if the query succeeded</returns>
        public static bool UpdateMilestoneProgression(int milestoneID, string username, int? progression, DateTime? completionDate)
        {
            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return false;
            }
            string statement;
            if (completionDate != null)
            {
                statement = $"update MilestoneProgressie set HuidigeProgressie = @progression, AfrondDatum = @completionDate where MilestoneID = @MIlestoneID and LeerlingGebruikersnaam = @Username";

            }
            else
            {
                statement = $"update MilestoneProgressie set HuidigeProgressie = @progression where MilestoneID = @MIlestoneID and LeerlingGebruikersnaam = @Username";
            }

            SqlCommand command = new(statement, connection);

            if (completionDate != null)
            {
                SqlParameter completionDateParam = new("@completionDate", System.Data.SqlDbType.Date);
                completionDateParam.Value = completionDate;
                command.Parameters.Add(completionDateParam);
            }
            SqlParameter milestoneIdParam = new("@MilestoneID", System.Data.SqlDbType.Int);
            milestoneIdParam.Value = milestoneID;
            command.Parameters.Add(milestoneIdParam);

            SqlParameter usernameParam = new("@Username", System.Data.SqlDbType.VarChar, 45);
            usernameParam.Value = username;
            command.Parameters.Add(usernameParam);

            SqlParameter progressionParam = new("@progression", System.Data.SqlDbType.Int);
            progressionParam.Value = progression;
            command.Parameters.Add(progressionParam);

            command.Prepare();
            return command.ExecuteNonQuery() == 1; // If amount of rows affected is 1
        }

        /// <summary>
        /// Function that returns a list of all made exercises in the form of their ID's. This is used to update the "make different exercises" type milestones
        /// </summary>
        /// <param name="username">Username of the current student</param>
        /// <returns>A list of Exercise IDs of exercises the student made</returns>
        public static List<int> GetMadeExercisesID(string username, int exerciseID, int attemptNR)
        {
            List<int> list = new();

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return list;
            }

            string statement = $"SELECT OefeningID FROM Statistieken WHERE LeerlingGebruikersnaam = @LeerlingGebruikersnaam AND NOT (OefeningID = @OefeningID AND PogingNR = @PogingNR);";
            SqlCommand sqlCmd = new SqlCommand(statement, connection);

            SqlParameter studentIdParam = new SqlParameter("@LeerlingGebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            studentIdParam.Value = username;
            sqlCmd.Parameters.Add(studentIdParam);

            SqlParameter exerciseIDParam = new SqlParameter("@OefeningID", System.Data.SqlDbType.Int);
            exerciseIDParam.Value = exerciseID;
            sqlCmd.Parameters.Add(exerciseIDParam);

            SqlParameter attemptNRParam = new SqlParameter("@PogingNR", System.Data.SqlDbType.Int);
            attemptNRParam.Value = attemptNR;
            sqlCmd.Parameters.Add(attemptNRParam);

            sqlCmd.Prepare();

            SqlDataReader reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                int ordOefeningID = reader.GetOrdinal("OefeningID");
                int oefeningID = reader.GetInt32(ordOefeningID);
                list.Add(oefeningID);
            }
            return list;
        }
        /// <summary>
        /// Function that returns the scores of previous attempts. Used to update milestones
        /// </summary>
        /// <param name="username">The username of the current student</param>
        /// <returns>The previous scores of the student as a List<int> </returns>
        public static int GetMaxPreviousResults(string username, int oefeningID)
        {
            int maxScore = 0;

            SqlConnection? connection = DatabaseUtils.OpenDatabaseConnection();
            if (connection == null)
            {
                return maxScore;
            }

            string statement = $"select Max(Score) as Score from Statistieken where LeerlingGebruikersnaam = @LeerlingGebruikersnaam  and OefeningID = @OefeningID and PogingNR < (select max(PogingNR) from Statistieken where LeerlingGebruikersnaam = @LeerlingGebruikersnaam and OefeningID = @OefeningID);";
            SqlCommand sqlCmd = new SqlCommand(statement, connection);

            SqlParameter studentIdParam = new SqlParameter("@LeerlingGebruikersnaam", System.Data.SqlDbType.VarChar, 45);
            studentIdParam.Value = username;
            sqlCmd.Parameters.Add(studentIdParam);

            SqlParameter exerciseIdParam = new SqlParameter("@OefeningID", System.Data.SqlDbType.Int);
            exerciseIdParam.Value = oefeningID;
            sqlCmd.Parameters.Add(exerciseIdParam);

            sqlCmd.Prepare();

            SqlDataReader reader = sqlCmd.ExecuteReader();
            if (reader.Read())
            {
                try
                {
                    int ordScore = reader.GetOrdinal("Score");
                    maxScore = reader.GetInt32(ordScore);
                }
                catch (Exception)
                {
                    maxScore = 0;
                }
            }
            return maxScore;
        }
        /// <summary>
        /// Function that updates the milestones after an exercise is completed. Saves the updated milestone in the DB
        /// </summary>
        /// <param name="result">The result of the made exercise</param>
        /// <returns>A list of MilestoneModels that have been completed because of this update</returns>
        public static List<MilestoneModel> UpdateMilestones(ExerciseResultModel result)
        {
            List<MilestoneModel> completedMilestones = new();
            List<MilestoneModel> currentIncompleteMilestones = new();//all the milestones that are not complete before the update happens
            //constructs the incomplete milestones
            foreach (Dictionary<string, Object> data in MilestoneUtils.GetIncompleteMilestones(UserUtils.Username))
            {
                currentIncompleteMilestones.Add(ConstructorViewModel.ConstructMilestone(data));
            }
            List<int> madeExercises = MilestoneUtils.GetMadeExercisesID(UserUtils.Username, result.ExerciseId, result.Attempt);
            //updates the milestones
            foreach (MilestoneModel milestone in currentIncompleteMilestones)
            {
                if (milestone.GetType().Equals(typeof(ScoreMilestoneModel)))
                {
                    ScoreMilestoneModel tempMilestone = (ScoreMilestoneModel)milestone;
                    if (MilestoneUtils.GetMaxPreviousResults(UserUtils.Username, result.ExerciseId) < tempMilestone.ScoreThreshold && result.Score > tempMilestone.ScoreThreshold)
                    {
                        milestone.Progression += 1;
                    }
                }
                else
                {
                    switch (milestone.Type)
                    {
                        case MilestoneType.Characters:
                            {//keys typed
                                milestone.Progression += result.Keys;
                            }
                            break;
                        case MilestoneType.AmountOfExercises:
                            {//made exercises
                                if (!madeExercises.Contains(result.ExerciseId))
                                {
                                    milestone.Progression += 1;
                                }
                            }
                            break;
                    }
                }
                //saves the updated milestone in the DB
                MilestoneUtils.UpdateMilestoneProgression(milestone.Id, UserUtils.Username, milestone.Progression, milestone.CompletionDate);
                //adds the completed milestones to the completion list
                if (milestone.CompletionDate != null)
                {

                    completedMilestones.Add(milestone);
                }
            }
            return completedMilestones;
        }
    }
}
