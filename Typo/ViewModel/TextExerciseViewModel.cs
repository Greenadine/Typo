using DiffPlex;
using System;
using System.Windows.Threading;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public class TextExerciseViewModel : PropertyChangable
    {
        public static ExerciseResultModel? Result { get; set; }

        public Command ConfirmCommand { get; set; }
        public Command CancelCommand { get; set; }

        private static string _input = string.Empty;

        public static string Input
        {
            get => _input;
            set => _input = value;
        }

        private TextExerciseModel _exercise;

        public TextExerciseModel Exercise
        {
            get => _exercise;
            set => _exercise = value;
        }

        private int _timeInSeconds;

        public int TimeInSeconds
        {
            get => _timeInSeconds;
            set
            {
                _timeInSeconds = value;
                DisplayTimer = MakeTimerReadable(TimeInSeconds);
            }
        }

        private string _displayTimer = "00:00";

        public string DisplayTimer
        {
            get => _displayTimer;
            set
            {
                _displayTimer = value;
                OnPropertyChanged(nameof(DisplayTimer));
            }
        }

        private DispatcherTimer _timer;

        public TextExerciseViewModel()
        {
            if (ExerciseUtils.SelectedExercise == null)
            {
                throw new NullReferenceException("Error: exercise is null");
            }
            _exercise = (TextExerciseModel)ExerciseUtils.SelectedExercise;

            ConfirmCommand = new Command(FinishExercise, null);
            CancelCommand = new Command(CancelExercise, null);

            // Initialize game variables
            _timeInSeconds = 0;

            // Initialize and start timer
            _timer = new DispatcherTimer {
                Interval = new TimeSpan(0, 0, 1) // Set interval to 1 second
            };
            _timer.Tick += PassTime; // Link timer tick method
            _timer.Start();
        }

        /// <summary>
        /// Increases the time spent by 1 second.
        /// </summary>
        private void PassTime(object? s, EventArgs e)
        {
            TimeInSeconds++;
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        private void StopTimer()
        {
            _timer.Stop();
            _timer.Tick -= PassTime;
        }

        /// <summary>
        /// Canncels the exercise.
        /// </summary>
        public void CancelExercise()
        {
            StopTimer();
            App.CurrentWindow = new StudentHomeView();
        }

        /// <summary>
        /// Finishes up the exercise.
        /// </summary>
        public void FinishExercise()
        {
            StopTimer();

            int amountWrong = GetAmountWrong(Input, _exercise.Text);
            int amountCorrect = _exercise.Text.Length - amountWrong;
            int score = CalculateScore(amountCorrect, _exercise.Text.Length);
            double keysPerSecond = 0; // TODO implement
            Result = new(UserUtils.Username, _exercise.Id, ExerciseResultUtils.GetNextAttempt(UserUtils.Username, _exercise.Id), score, _timeInSeconds, amountCorrect, amountWrong, keysPerSecond, amountCorrect);
            ExerciseResultUtils.SaveResult(Result);

            App.CurrentWindow = new TextExerciseResultsView();
        }

        /// <summary>
        /// Gets the amount of errors in the entered text opposed to the expected text.
        /// </summary>
        /// <param name="input">The input text.</param>
        /// <param name="expected">The expected (original) text.</param>
        /// <returns>The amount of errors made in the input text.</returns>
        private int GetAmountWrong(string input, string expected)
        {
            var differ = new Differ();
            var charDiff = differ.CreateCharacterDiffs(expected, input, false);
            int amountWrong = 0;

            if (charDiff.DiffBlocks.Count == 0)
            {
                return amountWrong;
            }

            foreach (var diffBlock in charDiff.DiffBlocks)
            {
                amountWrong += diffBlock.DeleteCountA;
                if (diffBlock.DeleteCountA != diffBlock.InsertCountB)
                {
                    amountWrong += diffBlock.InsertCountB;
                }
            }
            return amountWrong;
        }

        /// <summary>
        /// Calculates the score.
        /// </summary>
        /// <param name="amountCorrect">The amount of correct characters.</param>
        /// <param name="expectedLength">The length of the expected (original) text.</param>
        /// <returns>The score.</returns>
        private int CalculateScore(int amountCorrect, int expectedLength)
        {
            int result = (int)Math.Floor((1000 - _timeInSeconds) * GetRatioCorrect(amountCorrect, expectedLength));
            return result;
        }

        /// <summary>
        ///     Calculates the ratio of correct words over the total amount of words in the assignment.
        /// </summary>
        ///
        /// <returns>
        ///     The ratio of correct words.
        /// </returns>
        private double GetRatioCorrect(int amountCorrect, int expectedLength)
        {
            return (double)amountCorrect / expectedLength;
        }

        /// <summary>
        /// Creates a user-readable string of the timer.
        /// </summary>
        /// <param name="timeInSeconds">The time spent making the exercise, in seconds.</param>
        /// <returns>The formatted timer.1</returns>
        public string MakeTimerReadable(int timeInSeconds)
        {
            int minutes = timeInSeconds / 60; // aantal seconden delen door 60 geeft het aantal minuten
            int seconds = timeInSeconds % 60; // aantal seconden modulo 60 geeft het aantal seconden - alle seconden die binnen een minuut vallen

            string ReadableTimer;

            // Minutes
            if (minutes < 10)
            {
                ReadableTimer = '0' + minutes.ToString();
            }
            else
            {
                ReadableTimer = minutes.ToString();
            }

            // Seconds
            if (seconds < 10)
            {
                ReadableTimer += ":0" + seconds;
            }
            else
            {
                ReadableTimer += ":" + seconds;
            }
            return ReadableTimer;
        }
    }
}