using System;

namespace Typo.Model.Exercise
{
    public class ExerciseResultModel
    {
        // Properties
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public DateTime Date { get; set; }
        public string StudentUsername { get; set; }
        public int Attempt { get; set; }
        public int Score { get; set; }
        public int AmountCorrect { get; set; }
        public int AmountWrong { get; set; }
        public int TimeInSeconds { get; set; }
        public int? WordsDone { get; set; }
        public double KeysPerSecond { get; set; }
        public int Keys { get; set; }

        /// <summary>
        /// Save result constructor. Used after an exercise is completed.
        /// </summary>
        public ExerciseResultModel(string studentUsername, int exerciseId, int attempt, int score, int timeInSeconds, int amountCorrect, int amountWrong, double keysPerSecond, int keys) : this(studentUsername, exerciseId, "", attempt, score, timeInSeconds, amountCorrect, amountWrong, keysPerSecond, keys, DateTime.Now) { }

        /// <summary>
        /// Statics view constructor. Used to display the results in the StudentStatisticsView.
        /// </summary>
        public ExerciseResultModel(string studentUsername, int exerciseId, string exerciseName, int attempt, int score, int timeInSeconds, int amountCorrect, int amountWrong, double keysPerSecond, DateTime date) : this(studentUsername, exerciseId, exerciseName, attempt, score, timeInSeconds, amountCorrect, amountWrong, keysPerSecond, 0, date) { }

        /// <summary>
        /// Constructor used in the Marathon mode
        /// </summary>
        public ExerciseResultModel(string studentUsername, int exerciseId, int attempt, int score, int timeInSeconds, int amountCorrect, int amountWrong, double keysPerSecond, int keys, int wordsDone) : this(studentUsername, exerciseId, "", attempt, score, timeInSeconds, amountCorrect, amountWrong, keysPerSecond, keys, DateTime.Now)
        {
            WordsDone = wordsDone;
        }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public ExerciseResultModel(string studentUsername, int exerciseId, string exerciseName, int attempt, int score, int timeInSeconds, int amountCorrect, int amountWrong, double keysPerSecond, int keys, DateTime date)
        {
            ExerciseId = exerciseId;
            StudentUsername = studentUsername;
            ExerciseName = exerciseName;
            Attempt = attempt;
            Date = date;
            Score = score;
            AmountCorrect = amountCorrect;
            AmountWrong = amountWrong;
            TimeInSeconds = timeInSeconds;
            KeysPerSecond = keysPerSecond;
            Keys = keys;
        }

        /// <summary>
        /// Calculates and returns the percentage of correct answers over the total amount.
        /// </summary>
        /// <returns>Percentage of correct answers.</returns>
        public int GetPercentage()
        {
            int percentage = (int)((double)AmountCorrect / (AmountCorrect + AmountWrong) * 100);
            return percentage;
        }

        /// <summary>
        /// Converts the time to the mm:ss format.
        /// </summary>
        /// <returns>The time formatted as a string in the format 'HH:mm'.</returns>
        public string TimeToString()
        {
            int minutes = TimeInSeconds / 60;
            int seconds = TimeInSeconds % 60;
            string output = string.Empty;

            if (minutes < 10)
            {
                output += "0";
            }

            output += minutes + ":";

            if (seconds < 10)
            {
                output += 0;
            }

            output += seconds;
            return output;
        }
    }
}