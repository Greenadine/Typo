using System.Windows;
using Typo.ViewModel;

namespace Typo
{
    /// <summary>
    /// Interaction loagic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Window _currentWindow = Current.MainWindow;

        private static Window? _currentDialog = null;

        public static Window CurrentWindow
        {
            get => _currentWindow;
            set
            {
                // Show new screen, and close previous
                value.Show();
                _currentWindow.Close();

                _currentWindow = value; // Set as new current window

                // Update the view model
                if (_currentWindow.DataContext is IViewModel vm)
                {
                    vm.Update();
                }
            }
        }

        public static Window? CurrentDialog
        {
            get => _currentDialog;
            set
            {
                _currentDialog?.Close();
                _currentDialog = value;
                if (_currentDialog != null)
                {
                    _currentDialog.Owner = _currentWindow;
                    _currentDialog.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Sets the owner of the provided window to the current application window, and then shows it as a dialog.
        /// </summary>
        /// <param name="window">The window to show as a dialog.</param>
        public static void ShowDialog(Window window)
        {
            window.Owner = CurrentWindow;
            window.ShowDialog();
        }

        /// <summary>
        /// Shows a message box with the provided message.
        /// </summary>
        /// <param name="message">The message to show in the message box.</param>
        /// <param name="caption">The caption for the message box.</param>
        /// <returns>The MessageBoxResult.</returns>
        public static MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons)
        {
            return MessageBox.Show(CurrentWindow, message, caption, buttons);
        }
    }
}