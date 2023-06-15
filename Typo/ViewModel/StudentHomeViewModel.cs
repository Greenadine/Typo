using System.Collections.Generic;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Model.Milestone;
using Typo.Utils;
using Typo.View;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class StudentHomeViewModel : PropertyChangable, IViewModel, IHelp
    {
        // Sidebar
        public StudentSidebarView Sidebar { get; set; }

        private List<ExerciseModel>? _QuickExercises;
        private ExerciseModel? _selected;
        private string? _playButtonContent = "Selecteer een oefening";
        public List<MilestoneModel> MilestoneList { get; set; }

        public ExerciseModel? SelectedExercise
        {
            get => _selected;
            set
            {
                _selected = value;
                ChangePlayButton();
                ExerciseUtils.SelectedExercise = value;
            }
        }

        public List<ExerciseModel>? QuickExercises { get => _QuickExercises; set => _QuickExercises = value; }

        public string? PlayButtonContent
        {
            get => _playButtonContent;
            set
            {
                _playButtonContent = value;
                OnPropertyChanged("");
            }
        }

        public Command PlayCommand { get; set; }

        public StudentHomeViewModel()
        {
            Sidebar = new();

            QuickExercises = GetQuickAccessExercises();
            PlayCommand = new(() => StartExercise(SelectedExercise));
            MilestoneList = GetMilestones();
        }

        /// <summary>
        /// Changes the text of the play button.
        /// </summary>
        private void ChangePlayButton()
        {
            if (SelectedExercise == null)
            {
                PlayButtonContent = "Selecteer een oefening";
                return;
            }
            PlayButtonContent = "Start " + SelectedExercise.Name;
        }

        /// <summary>
        /// Gets the incomplete milestones of the student from the database.
        /// </summary>
        /// <returns>A List containing the incomplete milestones.</returns>
        private List<MilestoneModel> GetMilestones()
        {
            List<MilestoneModel> output = new();
            foreach (Dictionary<string, object> data in MilestoneUtils.GetIncompleteMilestones(UserUtils.Username))
            {
                output.Add(ConstructorViewModel.ConstructMilestone(data));
            }
            return output;
        }

        /// <summary>
        /// Gets the exercises to display in the quick access list.
        /// </summary>
        /// <returns>A List containing exercises to display.</returns>
        private List<ExerciseModel> GetQuickAccessExercises()
        {
            List<ExerciseModel> exercises = new();

            foreach (var data in ExerciseUtils.GetExercisesData())
            {
                exercises.Add(ConstructorViewModel.ConstructExercise(data));
            }
            return exercises;
        }

        /// <summary>
        /// Starts an exercise.
        /// </summary>
        /// <param name="exerciseModel">The exercise to starts.</param>
        public void StartExercise(ExerciseModel? exerciseModel)
        {
            if (exerciseModel == null)
            {
                return;
            }

            App.CurrentWindow = exerciseModel.Mode switch {
                Mode.Words => new ExecuteWordsExerciseView(),
                Mode.Text => new ExecuteTextExerciseView(),
                Mode.Marathon => new ExecuteMarathonExerciseView(),
                _ => throw new System.NotImplementedException()
            };
        }

        /// <summary>
        /// Returns the help information for this window.
        /// </summary>
        /// <returns>The WindowHelpModel for this window.</returns>
        public WindowHelpModel GetHelp()
        {
            return new("Welkom bij Typo!",
                "Leuk dat je er bent!\nOm meteen van start te gaan, kies je eerst een oefening om te maken door erop te klikken. " +
                "Daarna kan je starten door te klikken op de gele knop.\n\n" +
                "Je leraar/lerares zet oefeningen voor je klaar om te maken. Je kan je gemaakte oefeningen terug zien in het \"Statistieken\" menu. " +
                "Je kan ook kijken hoe goed je het doet in het \"Milestones\" menu.\nVeel leerplezier!");
        }

        /// <summary>
        /// Updates the nav button backgrounds based on the current screen.
        /// </summary>
        public void Update()
        {
            // Update sidebar view model
            if (Sidebar.DataContext is IViewModel vm)
            {
                vm.Update();
            }
        }
    }
}
