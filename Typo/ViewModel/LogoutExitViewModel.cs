using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public class LogoutExitViewModel
    {
        public Command ExitCommand { get; private set; }
        public Command LogoutCommand { get; private set; }
        public Command CancelCommand { get; private set; }

        public LogoutExitViewModel()
        {
            ExitCommand = Commands.EXIT;
            LogoutCommand = new(Logout);
            CancelCommand = new(() => App.CurrentDialog = null);
        }

        /// <summary>
        /// Logs the current user out, and navigates the application to the login screen.
        /// </summary>
        private void Logout()
        {
            UserUtils.Logout();
            App.CurrentWindow = new LoginView();
        }
    }
}
