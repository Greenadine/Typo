using System.Collections.Generic;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Utils;
using Typo.View;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class TeacherHomeViewModel : PropertyChangable, IViewModel, IHelp
    {
        // Sidebar
        public TeacherSidebarView Sidebar { get; set; }

        #region Binding properties
        private List<ExerciseModel> _exercises;
        public List<ExerciseModel> Exercises
        {
            get => _exercises;
            private set
            {
                _exercises = value;
                OnPropertyChanged(nameof(Exercises));
            }
        }
        #endregion

        public Command GoToAddExerciseCommand { get; set; }

        public TeacherHomeViewModel()
        {
            Sidebar = new();

            GoToAddExerciseCommand = new(GoToAddExercise);

            _exercises = GetExercises();
        }

        /// <summary>
        /// Navigates the application to the view for adding new exercises.
        /// </summary>
        private void GoToAddExercise()
        {
            App.CurrentWindow = new AddExerciseView();
        }

        /// <summary>
        /// Gets the exercises from the database.
        /// </summary>
        /// <returns>A list containing the exercises from the database.</returns>
        private static List<ExerciseModel> GetExercises()
        {
            List<ExerciseModel> exercises = new();

            foreach (Dictionary<string, object> exerciseData in ExerciseUtils.GetExercisesData())
            {
                exercises.Add(ConstructorViewModel.ConstructExercise(exerciseData));
            }

            return exercises;
        }

        /// <summary>
        /// Returns the help information for this window.
        /// </summary>
        /// <returns>The WindowHelpModel for this window.</returns>
        public WindowHelpModel GetHelp()
        {
            return new("Welkom bij Typo!", "Dit is uw dashboard. Vanaf hier kunt u al uw taken voor uw klas uitvoeren. " +
                "Klik op één van de getoonde knoppen om de beschreven taak uit te voeren.");
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
    }
}
