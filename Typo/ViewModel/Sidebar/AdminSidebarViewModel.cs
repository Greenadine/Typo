using System.Windows.Media;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel.Sidebar
{
    public class AdminSidebarViewModel : SidebarViewModel
    {
        #region Binding properties
        private string _adminName;
        public string AdminName
        {
            get => _adminName;
            set
            {
                _adminName = value;
                OnPropertyChanged(nameof(AdminName));
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

        public AdminSidebarViewModel()
        {
            _adminName = GetAdminName();

            _homeButtonBackground = new SolidColorBrush(_navButtonBackground);

            NavigateToHomeCommand = new(() => App.CurrentWindow = new AdminHomeView());
        }

        /// <summary>
        /// Gets the name of the student that's currently logged in.
        /// </summary>
        /// <returns>The name of the student that's currently logged in.</returns>
        private static string GetAdminName()
        {
            return UserUtils.GetCurrentAdminName();
        }

        /// <summary>
        /// Updates the nav button backgrounds based on the current screen.
        /// </summary>
        public override void Update()
        {
            HomeButtonBackground = SetButtonBackground(typeof(AdminHomeView));
        }
    }
}
