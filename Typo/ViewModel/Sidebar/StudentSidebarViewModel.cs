using System.Windows;
using System.Windows.Media;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel.Sidebar
{
    public class StudentSidebarViewModel : SidebarViewModel
    {
        #region Binding properties
        private string _studentName;
        public string StudentName
        {
            get => _studentName;
            private set
            {
                _studentName = value;
                OnPropertyChanged(nameof(StudentName));
            }
        }
        private Brush _homeButtonBackground;
        public Brush HomeButtonBackground
        {
            get => _homeButtonBackground;
            private set
            {
                _homeButtonBackground = value;
                OnPropertyChanged(nameof(HomeButtonBackground));
            }
        }
        private Brush _exercisesButtonBackground;
        public Brush ExercisesButtonBackground
        {
            get => _exercisesButtonBackground;
            private set
            {
                _exercisesButtonBackground = value;
                OnPropertyChanged(nameof(ExercisesButtonBackground));
            }
        }
        private Brush _statisticsButtonBackground;
        public Brush StatisticsButtonBackground
        {
            get => _statisticsButtonBackground;
            private set
            {
                _statisticsButtonBackground = value;
                OnPropertyChanged(nameof(StatisticsButtonBackground));
            }
        }
        private Brush _milestonesButtonBackground;
        public Brush MilestonesButtonBackground
        {
            get => _milestonesButtonBackground;
            private set
            {
                _milestonesButtonBackground = value;
                OnPropertyChanged(nameof(MilestonesButtonBackground));
            }
        }
        #endregion

        #region Commands
        public Command NavigateToHomeCommand { get; private set; }
        public Command NavigateToExercisesCommand { get; private set; }
        public Command NavigateToStatisticsCommand { get; private set; }
        public Command NavigateToMilestonesCommand { get; private set; }
        #endregion
        public StudentSidebarViewModel()
        {
            _studentName = GetStudentName();

            _homeButtonBackground = new SolidColorBrush(_navButtonBackground);
            _exercisesButtonBackground = new SolidColorBrush(_navButtonBackground);
            _statisticsButtonBackground = new SolidColorBrush(_navButtonBackground);
            _milestonesButtonBackground = new SolidColorBrush(_navButtonBackground);

            NavigateToHomeCommand = new(() => App.CurrentWindow = new StudentHomeView());
            NavigateToExercisesCommand = new(() => App.CurrentWindow = new StudentExercisesView());
            NavigateToStatisticsCommand = new(() => App.CurrentWindow = new StudentStatisticsView());
            NavigateToMilestonesCommand = new(() => App.CurrentWindow = new StudentMilestonesView());
        }

        /// <summary>
        /// Gets the name of the student that's currently logged in.
        /// </summary>
        /// <returns>The name of the student that's currently logged in.</returns>
        private static string GetStudentName()
        {
            return UserUtils.GetCurrentStudentName();
        }

        /// <summary>
        /// Updates the nav button backgrounds based on the current screen.
        /// </summary>
        public override void Update()
        {
            HomeButtonBackground = SetButtonBackground(typeof(StudentHomeView));
            ExercisesButtonBackground = SetButtonBackground(typeof(StudentExercisesView));
            StatisticsButtonBackground = SetButtonBackground(typeof(StudentStatisticsView));
            MilestonesButtonBackground = SetButtonBackground(typeof(StudentMilestonesView));

            if (App.CurrentWindow.DataContext is IHelp help)
            {
                HelpVisibility = Visibility.Visible;
                HelpViewModel.Help = help.GetHelp();
            }
        }
    }
}
