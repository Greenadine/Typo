using Renci.SshNet.Common;
using System;
using System.Windows;
using Typo.Model;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public class LoginViewModel : PropertyChangable, IViewModel
    {
        // Variables
        private string _username = string.Empty;
        private string _errorMessage = string.Empty;

        // Properties
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // Commands
        public Command LoginCommand { get; private set; }
        public Command ExitCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new(Login);
            ExitCommand = Commands.EXIT;

            InitializeSsh();
        }

        /// <summary>
        /// Initializes the connection to the SSH.
        /// </summary>
        public void InitializeSsh()
        {
            SshUtils.client.ErrorOccurred += SshException;

            SshUtils.Connect(); // Attempt to establish the SSH connection
        }

        /// <summary>
        /// Attempts to login the user with the provided username.
        /// </summary>
        /// <param name="username">The username to login with.</param>
        /// <returns>'true' if the login was successful, 'false' otherwise.</returns>
        private void Login()
        {
            OnPropertyChanged(nameof(Username));
            // If the login failed due to an invalid username

            // If no connection is currently active, attempt to make another connection
            if (!SshUtils.client.IsConnected)
            {
                SshUtils.Connect();
            }

            // If the SSH connection could not be established
            if (!SshUtils.client.IsConnected)
            {
                ErrorMessage = "Geen verbinding";
                return;
            }

            // Attempt to login with the provided user
            if (!UserUtils.Login(Username))
            {
                ErrorMessage = "Incorrecte gebruikersnaam";
                return;
            }

            // Open home view based on user type
            App.CurrentWindow = UserUtils.UserType switch
            {
                UserType.Student => new StudentHomeView(),
                UserType.Teacher => new TeacherHomeView(),
                UserType.Admin => new AdminHomeView(),
                _ => throw new Exception("Cannot redirect user to home: UserType is None or unknown."),
            };
        }

        private void SshException(object? sender, ExceptionEventArgs e)
        {
            ErrorMessage = "Geen verbinding";
        }

        public void Update()
        {
            // Do nothing
        }
    }
}
