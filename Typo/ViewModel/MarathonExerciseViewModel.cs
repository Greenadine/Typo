using System;
using System.IO;
using System.Windows.Threading;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public class MarathonExerciseViewModel : PropertyChangable
    {
        public static ExerciseResultModel? Result { get; set; }

        public MarathonExerciseModel? Exercise { get; set; } // Reference to assignment that's being done

        #region Binding properties
        public string? ExerciseName { get; set; }
        private string _currentWord = string.Empty;
        public string CurrentWord
        {
            get => _currentWord;
            set
            {
                _currentWord = value;
                OnPropertyChanged(nameof(CurrentWord));
            }
        }

        private string _timerString = string.Empty;
        public string TimerString
        {
            get { return _timerString; }
            set
            {
                _timerString = value;
                OnPropertyChanged(nameof(TimerString));
            }
        }

        private string _textboxInput = string.Empty;
        public string TextboxInput
        {
            get { return _textboxInput; }
            set
            {
                _textboxInput = value;
                OnPropertyChanged(nameof(TextboxInput));
            }
        }

        private int _correctCounter = 0;
        public int CorrectCounter
        {
            get { return _correctCounter; }
            set
            {
                _correctCounter = value;
                OnPropertyChanged(nameof(CorrectCounter));
            }
        }

        private int _incorrectCounter = 0;
        public int IncorrectCounter
        {
            get { return _incorrectCounter; }
            set
            {
                _incorrectCounter = value;
                OnPropertyChanged(nameof(IncorrectCounter));
            }
        }

        private int _wordsDone = 0;
        public int WordsDone
        {
            get => _wordsDone;
            set
            {
                _wordsDone = value;
                OnPropertyChanged(nameof(WordsDone));
            }
        }

        private int _currentStreak = 0; // Current streak of correct words
        public int CurrentStreak
        {
            get { return _currentStreak; }
            set
            {
                _currentStreak = value;
                OnPropertyChanged(nameof(CurrentStreak));
            }
        }
        #endregion

        #region Commands
        public Command CancelCommand { get; set; }
        public Command CheckInputCommand { get; set; }
        #endregion

        // Word vars
        private readonly Random _random = new(); // Random number generator for word index
        private int _currentWordIndex; // Index of the current word within the assignment's words list

        // Time vars
        private readonly DispatcherTimer _timer; // Timer for the game

        private double _countdownTime; // Time remaining in the game
        public double CountdownTime
        {
            get => _countdownTime;
            set
            {
                _countdownTime = value;
                OnPropertyChanged(nameof(CountdownTime));
            }
        }
        private double _elapsedTime; // Time elapsed in game
        public double ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                OnPropertyChanged(nameof(ElapsedTime));
            }
        }
        private int _keys; //variable that stores the amount of keys typed. Used for milestones
        private double _streakMultiplier; // Multiplier for bonus time based on current streak
        private double _difficultyMultiplier; // Multiplier for bonus time based on current streak
        private static double BonusTime => 1; // Bonus time to add for correct answers

        public MarathonExerciseViewModel()
        {
            if (ExerciseUtils.SelectedExercise == null)
            {
                throw new NullReferenceException("Error: SelectedExercise is null");
            }
            Exercise = (MarathonExerciseModel)ExerciseUtils.SelectedExercise;

            // Initialize timer
            _timer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(1) // Set interval to 1 second
            };
            _timer.Tick += PassTime; // Link timer tick method

            CancelCommand = new(CancelExercise, null);
            CheckInputCommand = new(CheckInput, null);

            // Initialize game variables
            if (Exercise != null)
            {
                ExerciseName = Exercise.Name; // Sets exercise name
            }

            _currentWordIndex = GetNextIndex(); // Initializes random index
            CurrentWord = GetCurrentWord(); // Initializes first word
            _correctCounter = 0; // Number of correct words typed
            _incorrectCounter = 0; // Number of incorrect words typed
            _countdownTime = 5; // Initial time limit
            _currentStreak = 0; // Number of consecutive correct words typed, resets to 0 after incorrect word
            _keys = 0;
            if (SshUtils.client.IsConnected)
            {
                _timer.Start(); // Start timer
            }
        }

        /// <summary>
        ///     Returns whether the given input matches the current word to be typed over.
        /// </summary>
        ///
        /// <param name="input">
        ///     The input to check for.
        /// </param>
        ///
        /// <returns>
        ///     <code>true</code> if the input matches the current word,
        ///     <code>false</code> otherwise.
        /// </returns>
        public void CheckInput()
        {
            OnPropertyChanged(nameof(TextboxInput));

            // If the input matches the current word
            if (GetCurrentWord().Equals(TextboxInput.Trim()))
            {
                CorrectCounter++; // Adds 1 to correct word counter
                CurrentStreak++; // Adds 1 to streakcounter
                _keys += CurrentWord.Length;//Adds the length of the word to _keys
                AddBonusTime(); // Adds bonus time
                string correctSound = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Sounds", "CorrectSoundTypo.wav"); // Creates a relative path in a string
                SoundUtils.PlayOnce(correctSound);  // plays the sound
            }
            // If the input does not match the current word
            else
            {
                IncorrectCounter++; // Adds 1 to incorrect word counter
                CurrentStreak = 0; // Resets streak counter
                string wrongSound = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Sounds", "WrongSoundTypo.wav"); // Creates a relative path in a string
                SoundUtils.PlayOnce(wrongSound);  // plays the sound
            }

            WordsDone++;
            _currentWordIndex = GetNextIndex(); // Go to the next random index of wordlist
            TextboxInput = "";
            GetNextIndex();
            CurrentWord = GetCurrentWord();// Go to the next word
        }

        /// <summary>
        ///     Event for when a second passes on the timer.
        /// </summary>
        private void PassTime(object? s, EventArgs e)
        {
            CountdownTime--;
            ElapsedTime++;

            if (CountdownTime <= 0)
            {
                FinishGame();
            }
        }

        /// <summary>
        /// Adds bonus time to _countdownTime, based on streak and difficulty
        /// </summary>
        private void AddBonusTime()
        {
            if (CountdownTime < 60)
            {
                _streakMultiplier = ReturnStreakMultiplier(_currentStreak); // Convert the int from streak to a multiplier
                _difficultyMultiplier = ReturnDifficultyMultiplier(); // Convert the int from difficulty to a multiplier
                _countdownTime +=
                    (_streakMultiplier * BonusTime) *
                    _difficultyMultiplier; // Adds bonus time, formula = (streakmultiplier * the bonus time) / difficulty multiplier
            }
        }

        /// <summary>
        ///     Takes the streak and returns it as a multiplier
        /// </summary>
        public double ReturnStreakMultiplier(int currentStreak)
        {
            int streak = currentStreak;
            switch (streak)
            {
                case 1:
                case 2:
                case 3:
                case 4: return 1.0;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9: return 1.25;
                default: return 1.5;
            }
        }

        /// <summary>
        /// Takes the difficulty of the exercise and returns a multiplier
        /// </summary>
        /// <returns>
        /// Input 1 = Easy
        /// Input 2 = Medium
        /// Input 3 = Hard
        /// Multiplier 1.0 when Easy
        /// Multiplier 0.75 when Medium
        /// Multiplier 0.5 when Hard
        /// </returns>
        public double ReturnDifficultyMultiplier()
        {
            switch (Exercise?.Difficulty ?? 0)
            {
                case Difficulty.Normal: return 0.75;
                case Difficulty.Hard: return 0.5;
                default: return 1.0;
            }
        }

        /// <summary>
        /// Gets a random index
        /// </summary>
        /// <returns>
        /// random int between 0 and wordlist.count
        /// </returns>
        private int GetNextIndex()
        {
            // Get a random index within the range of the word list
            int index = _random.Next(0, Exercise?.Words.Count - 1 ?? 0);

            // Return the index
            return index;
        }

        /// <summary>
        ///     Gets the current word to be typed over.
        /// </summary>
        ///
        /// <returns>
        ///     The current word to be typed over.
        /// </returns>
        private string GetCurrentWord()
        {
            return Exercise?.Words[_currentWordIndex] ?? "Index Error";
        }

        /// <summary>
        /// Finishes the game, creates and shows the score screen and saves the result to the database.
        /// </summary>
        public void FinishGame()
        {
            _timer.Stop();
            Result = CreateResultModel();
            ExerciseResultUtils.SaveResult(Result); // Save to database
            _timer.Tick -= PassTime;
            App.CurrentWindow = new MarathonExerciseResultsView(); // Open Score Screen
        }

        /// <summary>
        /// Create and show assignment result, and save it to database
        /// </summary>
        private ExerciseResultModel CreateResultModel()
        {
            if (Exercise == null)
            {
                throw new NullReferenceException("Error: Exercise is null");
            }
            int score = CalculateScore(CorrectCounter, WordsDone);
            int elapsedTime = (int)Math.Floor(ElapsedTime); // Round elapsed time down
            Result = new ExerciseResultModel(
                UserUtils.Username,
                Exercise.Id,
                ExerciseResultUtils.GetNextAttempt(UserUtils.Username, Exercise.Id),
                score,
                elapsedTime,
                CorrectCounter,
                IncorrectCounter,
                0,
                _keys,
                WordsDone);
            return Result;
        }

        /// <summary>
        /// Calculates the score
        /// </summary>
        /// <param name="amountCorrect">The amount of correct answers.</param>
        /// <param name="wordsDone">The amount of words that were typed over.</param>
        /// <returns>The score.</returns>
        public int CalculateScore(int amountCorrect, int wordsDone)
        {
            if (wordsDone == 0)
            {
                return 0;
            }
            //int score = (int)Math.Floor((0 + _elapsedTime) * GetRatioCorrect(amountCorrect, wordsDone));
            double ratio = GetRatioCorrect(amountCorrect, wordsDone);
            int score = (int)Math.Floor(ElapsedTime * ratio);
            return score;
        }

        /// <summary>
        ///     Calculates the ratio of correct words over the total amount of words in the assignment.
        /// </summary>
        ///
        /// <returns>
        ///     The ratio of correct words.
        /// </returns>
        private double GetRatioCorrect(int amountCorrect, int wordsDone)
        {
            return (double)amountCorrect / wordsDone;
        }

        /// <summary>
        /// Stops timer and exits to main menu
        /// </summary>
        public void CancelExercise()
        {
            _timer.Stop();
            _timer.Tick -= PassTime;
            App.CurrentWindow = new StudentHomeView();
        }
    }
}
