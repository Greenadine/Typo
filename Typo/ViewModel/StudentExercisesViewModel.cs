using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Utils;
using Typo.View;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class StudentExercisesViewModel : PropertyChangable, IViewModel, IHelp
    {
        // Sidebar
        public StudentSidebarView Sidebar { get; set; }

        public List<ExerciseModel>? Exercises { get; set; }

        public ExerciseModel? SelectedExercise
        {
            get => ExerciseUtils.SelectedExercise;
            set
            {
                ExerciseUtils.SelectedExercise = value;
                SelectedDifficulty = TranslateDifficulty(value?.Difficulty);
                SelectedMode = TranslateMode(value?.Mode);
                OnPropertyChanged(nameof(SelectedExercise));
            }
        }

        private string? _selectedExerciseLength = string.Empty;

        public string? SelectedExerciseLength
        {
            get => _selectedExerciseLength;
            set => _selectedExerciseLength = value;
        }

        private string? _selectedmode;

        public string? SelectedMode
        {
            get => _selectedmode;
            set => _selectedmode = value;
        }

        private string? _selectedDifficulty;

        public string? SelectedDifficulty
        {
            get => _selectedDifficulty;
            set => _selectedDifficulty = value;
        }

        private string _playButtonContent = "Selecteer een oefening";

        public string PlayButtonContent
        {

            get => _playButtonContent;
            set => _playButtonContent = value;
        }

        public Command PlayCommand { get; set; }

        public StudentExercisesViewModel()
        {
            Sidebar = new();

            Exercises = GetExercises();
            PropertyChanged += SelectedChanged;
            PlayCommand = new(PlayPressed, null);
        }

        /// <summary>
        /// Gets the exercises from the database.
        /// </summary>
        /// <returns>A list containing all the exercises from the database.</returns>
        private static List<ExerciseModel> GetExercises()
        {
            List<ExerciseModel> exercises = new List<ExerciseModel>();
            foreach (var data in ExerciseUtils.GetExercisesData())
            {
                exercises.Add(ConstructorViewModel.ConstructExercise(data));
            }
            return exercises;
        }

        /// <summary>
        /// Changes the text displayed in the play button.
        /// </summary>
        private void ChangePlayButton()
        {
            if (SelectedExercise == null)
            {
                PlayButtonContent = "Selecteer een oefening";
                return;
            }
            PlayButtonContent = "Start - " + SelectedExercise.Name;
        }

        /// <summary>
        /// Function for when the play button is pressed
        /// </summary>
        private void PlayPressed()
        {
            StartExercise(SelectedExercise);
        }

        /// <summary>
        /// Starts the provided exercise.
        /// </summary>
        /// <param name="exerciseModel">The exercise to start.</param>
        public static void StartExercise(ExerciseModel? exerciseModel)
        {
            if (exerciseModel == null)
            {
                return;
            }
            Window exerciseWindow = exerciseModel.Mode switch {
                Mode.Words => new ExecuteWordsExerciseView(),
                Mode.Text => new ExecuteTextExerciseView(),
                Mode.Marathon => new ExecuteMarathonExerciseView(),
                _ => throw new NotImplementedException()
            };

            exerciseWindow.Show();
        }

        /// <summary>
        /// Event for when the selected exercises is changed. 
        /// Changes the play button content to the name of the exercise, and updates the displayed details.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event.</param>
        /// <exception cref="NotImplementedException">If an unsupported exercise type is selected.</exception>
        public void SelectedChanged(object? sender, PropertyChangedEventArgs? e)
        {
            if (SelectedExercise == null)
            {
                return;
            }
            ChangePlayButton();

            if (SelectedExercise is TextExerciseModel textExercise)
            {
                SelectedExerciseLength = textExercise.Text.Length + " letters";
            }
            else if (SelectedExercise is WordsExerciseModel wordsExercise)
            {
                SelectedExerciseLength = wordsExercise.Words.Count + " woorden";
            }
            else if (SelectedExercise is MarathonExerciseModel marathonExercise)
            {
                SelectedExerciseLength = marathonExercise.Words.Count + " woorden";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the help information for this window.
        /// </summary>
        /// <returns>The WindowHelpModel for this window.</returns>
        public WindowHelpModel GetHelp()
        {
            return new("Hulp - Oefeningen maken",
                "Om een oefening te maken, moet je eerst ééntje selecteren van de lijst. " +
                "Klik op een oefening die je interessant lijkt, en klik dan op de gele knop om van start te gaan!");
        }

        /// <summary>
        /// Updates the nav button backgrounds based on the current screen.
        /// </summary>
        public void Update()
        {
            if (Sidebar.DataContext is IViewModel vm)
            {
                vm.Update();
            }
        }

        /// <summary>
        /// Gets a string representing the mode of an exercise for displaying purposes.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>The string representing the mode.</returns>
        public static string TranslateMode(Mode? mode)
        {
            return mode switch {
                Mode.Words => "Woordenmodus",
                Mode.Text => "Tekstmodus",
                Mode.Marathon => "Marathonmodus",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Gets a string represneting the difficulty of an exercise for displaying purposes.
        /// </summary>
        /// <param name="difficulty">The difficulty.</param>
        /// <returns>The string representing </returns>
        public static string TranslateDifficulty(Difficulty? difficulty)
        {
            return difficulty switch {
                Difficulty.Easy => "Makkelijk",
                Difficulty.Normal => "Gemiddeld",
                Difficulty.Hard => "Moeilijk",
                _ => string.Empty
            };
        }
    }
}