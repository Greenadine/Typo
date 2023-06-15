using System;
using System.Collections.Generic;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Model.Milestone;
using Typo.Model.User;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public class ConstructorViewModel
    {
        /// <summary>
        /// Constructs an ExerciseModel based on the provided data.
        /// </summary>
        /// <param name="data">The exercise data.</param>
        /// <returns>The constructed ExerciseModel.</returns>
        /// <exception cref="Exception">If an invalid exercise mode was found.</exception>
        public static ExerciseModel ConstructExercise(Dictionary<string, object> data)
        {
            int id = (int)data["OefeningID"];
            int classId = (int)data["KlasID"];
            string? name = (string)data["Naam"];
            Mode mode = (Mode)(int)data["Modus"];
            Difficulty difficulty = (Difficulty)(int)data["Moeilijkheid"];
            bool visibility = (bool)data["Zichtbaarheid"];
            string content = (string)data["Inhoud"];

            switch (mode)
            {
                case Mode.Words:
                    List<string> words = new(content.Split(';'));
                    return new WordsExerciseModel(id, classId, name, difficulty, visibility, words);

                case Mode.Text:
                    return new TextExerciseModel(id, classId, name, difficulty, visibility, content);

                case Mode.Marathon:
                    List<string> wordsMarathon = new(content.Split(';'));
                    return new MarathonExerciseModel(id, classId, name, difficulty, visibility, wordsMarathon);

                default:
                    throw new Exception($"Invalid mode '{mode}' found.");
            }
        }

        /// <summary>
        /// Constructs a StudentModel based on the provided data.
        /// </summary>
        /// <param name="data">The student data.</param>
        /// <returns>The constructed StudentModel.</returns>
        public static StudentModel ConstructStudent(Dictionary<string, object> data)
        {
            string username = (string)data["Gebruikersnaam"];
            string name = (string)data["Naam"];
            int? classId = null;
            if (data.ContainsKey("KlasID"))
            {
                classId = (int)data["KlasID"];
            }

            return new StudentModel(username, name, classId);
        }

        /// <summary>
        /// Constructs a TeacherModel based on the provided data.
        /// </summary>
        /// <param name="data">The teacher data.</param>
        /// <returns>The constructed TeacherModel.</returns>
        public static TeacherModel ConstructTeacher(Dictionary<string, object> data)
        {
            if (data == null || data.Count == 0)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return new(string.Empty, string.Empty, new());
            }
            string username = (string)data["Gebruikersnaam"];
            string name = (string)data["Naam"];
            List<int> classIds = (List<int>)data["Klassen"];

            return new TeacherModel(username, name, classIds);
        }

        /// <summary>
        /// Constructs a ClassModel based on the provided data.
        /// </summary>
        /// <param name="data">The student data.</param>
        /// <returns>The constructed StudentModel.</returns>
        public static ClassModel ConstructClass(Dictionary<string, object> data)
        {
            int classId = (int)data["KlasID"];
            string name = (string)data["Naam"];

            return new ClassModel(classId, name);
        }

        /// <summary>
        /// Constructs a ExerciseResultModel based on the provided data.
        /// </summary>
        /// <param name="data">The result data</param>
        /// <returns>The constructed ExerciseResultModel</returns>
        public static ExerciseResultModel ConstructResultModel(string username, Dictionary<string, object> data)
        {
            string studentUsername = username;
            int exerciseId = (int)data["OefeningID"];
            string exerciseName = (string)data["OefeningNaam"];
            int attempt = (int)data["PogingNR"];
            int score = (int)data["Score"];
            int timeInSeconds = (int)data["Tijd"];
            int amountCorrect = (int)data["HoeveelheidGoed"];
            int amountWrong = (int)data["HoeveelheidFout"];
            DateTime date = (DateTime)data["AfrondDatum"];
            double keysPerSecond = (double)Convert.ToDouble(data["ToetsenPerSeconde"]);
            return new ExerciseResultModel(studentUsername, exerciseId, exerciseName, attempt, score, timeInSeconds, amountCorrect, amountWrong, keysPerSecond, date);
        }

        /// <summary>
        /// Constructs a MilestoneModel based on the provided data
        /// </summary>
        /// <param name="data">The result data from the DB</param>
        /// <returns>The constructed MilestoneModel</returns>
        public static MilestoneModel ConstructMilestone(Dictionary<string, object> data)
        {
            int id = (int)data["MilestoneID"];
            string name = (string)data["Name"];
            string description = (string)data["Description"];
            string imagePath = (string)data["ImagePath"];
            int progression = (int)data["Progression"];
            int threshold = (int)data["Threshold"];

            DateTime? completionDate = null;
            if (data.ContainsKey("CompletionDate"))
            { //CompletionDate can be null
                completionDate = (DateTime)data["CompletionDate"];
            }

            int scoreThreshold;

            MilestoneType type;
            switch (data["Type"])
            { //converts the DB value to MilestoneType enum
                case 0:
                    type = MilestoneType.Characters;
                    return new MilestoneModel(id, name, description, type, imagePath, progression, threshold, completionDate);
                case 1:
                    type = MilestoneType.AmountOfExercises;
                    return new MilestoneModel(id, name, description, type, imagePath, progression, threshold, completionDate);
                case 2:
                    type = MilestoneType.AmountOfExercisesWithScore;
                    scoreThreshold = (int)data["ScoreThreshold"];
                    return new ScoreMilestoneModel(id, name, description, type, imagePath, progression, threshold, scoreThreshold, completionDate);
                default:
                    throw new ArgumentException("Error: Milestone type not supported");
            }
        }
    }
}