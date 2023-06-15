using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public class WordExerciseViewModel : PropertyChangable, IViewModel
    {
        public WordsExerciseModel Exercise { get; set; }
        private int _wordIndex;
        private readonly DispatcherTimer _timer;
        public int TimerInt { get; set; } = 0;
        public static ExerciseResultModel? Result { get; set; }

        #region Binding properties
        public string ExerciseName { get; set; }
        public string CurrentWord
        {
            get => _currentWord;
            set
            {
                _currentWord = value;
                OnPropertyChanged(nameof(CurrentWord));
            }
        }
        private string _currentWord = string.Empty;
        public string TimerString
        {
            get => _timerString;
            set
            {
                _timerString = value;
                OnPropertyChanged(nameof(TimerString));
            }
        }
        private string _timerString = string.Empty;
        public string TextboxInput
        {
            get => _textboxInput;
            set
            {
                _textboxInput = value;
                OnPropertyChanged(nameof(TextboxInput));
            }
        }
        private string _textboxInput = string.Empty;
        public int CorrectCounter
        {
            get => _correctCounter;
            set
            {
                _correctCounter = value;
                OnPropertyChanged(nameof(CorrectCounter));
            }
        }
        private int _correctCounter = 0;
        public int IncorrectCounter
        {
            get => _incorrectCounter;
            set
            {
                _incorrectCounter = value;
                OnPropertyChanged(nameof(IncorrectCounter));
            }
        }
        private int _incorrectCounter = 0;
        public string WordsToGo
        {
            get => _wordsToGo;
            set
            {
                _wordsToGo = value;
                OnPropertyChanged(nameof(WordsToGo));
            }
        }
        private string _wordsToGo = string.Empty;
        private int _keys;
        #endregion

        public Command CancelCommand { get; set; }
        public Command CheckInputCommand { get; set; }

        public WordExerciseViewModel()
        {
            if (ExerciseUtils.SelectedExercise == null)
            {
                throw new NullReferenceException("Error: Selected exercise is null");
            }
            Exercise = (WordsExerciseModel)ExerciseUtils.SelectedExercise;
            _timer = new();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += PassTime;

            CancelCommand = new Command(CancelExercise, null);
            CheckInputCommand = new Command(CheckInput, null);

            ExerciseName = Exercise.Name;
            CurrentWord = Exercise.Words[0];
            TimerInt = 0;
            UpdateTimerString();
            TextboxInput = "";
            CorrectCounter = 0;
            IncorrectCounter = 0;
            WordsToGo = "0/" + Exercise.Words.Count;
            _wordIndex = 0;
            _keys = 0;


            _timer.Start();
        }

        /// <summary>
        ///     Returns whether the given input matches the current word to be typed over.
        /// </summary>
        /// <returns>
        ///     <code>true</code> if the input matches the current word,
        ///     <code>false</code> otherwise.
        /// </returns>
        public void CheckInput()
        {
            OnPropertyChanged(nameof(TextboxInput));
            // If the input matches the current word
            if (CurrentWord.Equals(TextboxInput.Trim()))
            {
                CorrectCounter++;
                string correctSound = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Sounds", "CorrectSoundTypo.wav"); // Creates a relative path in a string
                SoundUtils.PlayOnce(correctSound);  // plays the sound
                _keys += TextboxInput.Trim().Length;
            }
            // If the input does not match the current word
            else
            {
                IncorrectCounter++;
                string wrongSound = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Sounds", "WrongSoundTypo.wav");// Creates a relative path in a string
                SoundUtils.PlayOnce(wrongSound);    // plays the sound
            }
            // Check if there are any words left in the assignment
            if (_wordIndex >= Exercise.Words.Count - 1)
            {
                FinishGame();
                return;
            }
            _wordIndex++;
            TextboxInput = string.Empty;
            UpdateWord();// Go to the next word
            UpdateWordsToGo();//updates the words to go counter
        }

        public void UpdateWord()
        {
            CurrentWord = Exercise.Words[_wordIndex];
        }

        /// <summary>
        ///     Finishes the game, creates and shows the score screen and saves the result to the database.
        /// </summary>
        public void FinishGame()
        {
            _timer.Stop();
            // Create and show assignment result, and save it to database
            int score = CalculateScore(CorrectCounter, Exercise.Words.Count());
            Result = new(UserUtils.Username, Exercise.Id, ExerciseResultUtils.GetNextAttempt(UserUtils.Username, Exercise.Id), score, TimerInt, CorrectCounter, IncorrectCounter, 0, _keys);
            // Create and show screen with results
            // Save to database
            ExerciseResultUtils.SaveResult(Result);
            // Close game screen
            _timer.Tick -= PassTime;
            App.CurrentWindow = new ScoreScreenView();
        }

        /// <summary>
        ///     Event for when a second passes on the timer.
        /// </summary>
        private void PassTime(object? s, EventArgs e)
        {
            TimerInt++;
            UpdateTimerString();
        }

        /// <summary>
        /// Cancels the exercise.
        /// </summary>
        public void CancelExercise()
        {
            StopTimer();
            App.CurrentWindow = new StudentHomeView();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        private void StopTimer()
        {
            _timer.Stop();
            _timer.Tick -= PassTime;
        }

        //Timer conversion functions
        /// <summary>
        /// Function to convert the Timer int to a string
        /// </summary>
        /// <returns>The timer time in mm:ss</returns>
        private void UpdateTimerString()
        {
            string output = "";
            int minutes = TimerInt / 60;
            int seconds = TimerInt % 60;
            if (minutes < 10)
            {
                output += "0";
            }
            output += minutes + ":";
            if (seconds < 10)
            {
                output += "0";
            }
            output += seconds;
            TimerString = output;
        }
        /// <summary>
        /// Updates the WordsToGo counter in the correct format
        /// </summary>
        private void UpdateWordsToGo()
        {
            int completed = CorrectCounter + IncorrectCounter;
            WordsToGo = completed + "/" + Exercise.Words.Count;
        }
        /// <summary>
        /// Calculates the score used in the result. Called when exercise is finishing
        /// </summary>
        /// <param name="amountCorrect"></param>
        /// <param name="expectedLength"></param>
        /// <returns></returns>
        public int CalculateScore(int amountCorrect, int expectedLength)
        {
            int result = (int)Math.Floor((1000 - TimerInt) * GetRatioCorrect(amountCorrect, expectedLength));
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

        public void Update()
        {
            // Do nothing
        }
    }
}