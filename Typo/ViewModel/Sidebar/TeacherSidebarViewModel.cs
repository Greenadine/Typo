using System.Windows.Media;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel.Sidebar
{
    public class TeacherSidebarViewModel : SidebarViewModel
    {
        #region Binding properties
        private string _teacherName;
        public string TeacherName
        {
            get => _teacherName;
            set
            {
                _teacherName = value;
                OnPropertyChanged(nameof(TeacherName));
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
        #endregion

        #region Commands
        public Command NavigateToHomeCommand { get; private set; }
        #endregion

        public TeacherSidebarViewModel()
        {
            _teacherName = GetTeacherName();

            _homeButtonBackground = new SolidColorBrush(_navButtonBackground);

            NavigateToHomeCommand = new(NavigateToHome);
        }

        /// <summary>
        /// Gets the name of the teacher that's currently logged in.
        /// </summary>
        /// <returns>The teacher's name.</returns>
        private static string GetTeacherName()
        {
            return UserUtils.GetCurrentTeacherName();
        }

        /// <summary>
        /// Navigates the application to the teacher home view.
        /// </summary>
        private static void NavigateToHome()
        {
            App.CurrentWindow = new TeacherHomeView();
        }

        /// <summary>
        /// Updates the nav button backgrounds based on the current screen.
        /// </summary>
        public override void Update()
        {
            HomeButtonBackground = SetButtonBackground(typeof(TeacherHomeView));
        }
    }
}
