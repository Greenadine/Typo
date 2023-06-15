using System.Windows;

namespace Typo.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enables focus on the username textbox when the element is loaded.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TextBoxLoaded(object sender, RoutedEventArgs e)
        {
            TextBox_Username.Focus();
        }
    }
}
