using System;
using System.Windows;
using System.Windows.Media;
using Typo.Model;
using Typo.View;

namespace Typo.ViewModel.Sidebar
{
    public abstract class SidebarViewModel : PropertyChangable, IViewModel
    {
        // For changing navigation button colors based on the currently-active window
        protected readonly Color _navButtonBackground = (Color)ColorConverter.ConvertFromString("#FF3E4369");
        protected readonly Color _navButtonActiveBackground = (Color)ColorConverter.ConvertFromString("#FF505580");

        private Visibility _helpVisibility = Visibility.Collapsed;
        public Visibility HelpVisibility
        {
            get => _helpVisibility;
            set
            {
                _helpVisibility = value;
                OnPropertyChanged(nameof(HelpVisibility));
            }
        }

        #region Commands
        public Command SettingsCommand { get; private set; }
        public Command ShowHelpCommand { get; protected set; }
        public Command LogoutExitCommand { get; private set; }
        #endregion

        protected SidebarViewModel()
        {
            SettingsCommand = new(ShowSettings);
            ShowHelpCommand = new(ShowHelp);
            LogoutExitCommand = new(ShowLogoutExit);
        }

        /// <summary>
        /// Shows the settings window as a dialog.
        /// </summary>
        private static void ShowSettings()
        {
            App.CurrentDialog = new SettingsView();
        }

        /// <summary>
        /// Shows the help dialog for this window.
        /// </summary>
        private static void ShowHelp()
        {
            if (App.CurrentWindow.DataContext is IHelp)
            {
                App.CurrentDialog = new HelpView();
            }
        }

        /// <summary>
        /// Shows the logout/exit window as a dialog.
        /// </summary>
        private static void ShowLogoutExit()
        {
            App.CurrentDialog = new LogoutExitView();
        }

        protected Brush SetButtonBackground(Type type)
        {
            if (App.CurrentWindow.GetType() == type)
            {
                return new SolidColorBrush(_navButtonActiveBackground);
            } else
            {
                return new SolidColorBrush(_navButtonBackground);
            }
        }

        public abstract void Update();
    }
}