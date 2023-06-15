using System.Windows;
using Typo.Model;
using Typo.Utils;
using Typo.View;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class AddAccountViewModel : PropertyChangable, IViewModel
    {
        // Sidebar
        public AdminSidebarView Sidebar { get; set; }

        // Variables

        private string _errorMessage = string.Empty;

        #region Binding properties
        public string Name { get; set; } = string.Empty;
        public int AccountType { get; set; }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        #endregion

        #region Commands
        public Command AddAccountCommand { get; set; }
        public Command ReturnCommand { get; set; }
        #endregion

        public AddAccountViewModel()
        {
            Sidebar = new();

            AddAccountCommand = new(AddAccount);
            ReturnCommand = new(Return);
        }
        /// <summary>
        /// Checks all the input of the view before adding a new account.
        /// </summary>
        private bool CheckInput()
        {
            if (Name.Equals(string.Empty))
            {
                SetErrorMessage("Vul een naam in");
                return false;
            }
            if (AccountType == -1)
            {
                SetErrorMessage("Vul een Accounttype in");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds a new Account to the database, based on the input of the view.
        /// </summary>
        private void AddAccount()
        {
            if (!CheckInput()) // check if there is an input.
            {
                return;
            }

            string Username = Name.Trim(); // Trim() so that there are no white space in the database.
            string name = Name.Trim();

            if (AccountType == 0) // Teacher
            {
                int AmountEqualName = StudentUtils.StudentCount(name); //Check how many other studends have the same name,  return int.
                Username += "L"; // indicator of a student.
                if (AmountEqualName > 0)
                {
                    Username += AmountEqualName.ToString(); // Add the amount of equal names to the end of the username.
                }

                if (AmountEqualName == -1) // -1 = database failure.
                {
                    SetErrorMessage("Database connectie gefaald");
                    return;
                }

                Username = Username.Replace(" ", ""); // remove whitespace between word: example Jan Smit, is now JanSmit.

                if (!StudentUtils.AddStudent(Username, name)) // If the student wasn't succesfully added.
                {
                    App.ShowMessageBox("Er is een fout opgetreden bij het aanmaken van een leerling.", "Fout", MessageBoxButton.OK);
                    return;
                }

                App.ShowMessageBox($"De leerling is aangemaakt.\n Gebruikersnaam: {Username}", "Success", MessageBoxButton.OK);
                Return(); // Return to home
            }

            if (AccountType == 1)  // Student
            {
                int AmountEqualName = TeacherUtils.TeacherCount(name); //Check how many other teachers have the same name,  return int.
                Username += "D"; // indicator of a teacher
                if (AmountEqualName > 0)
                {
                    Username += AmountEqualName.ToString(); // Add the amount of equal names to the end of the username.
                }
                if (AmountEqualName == -1) // -1 = database failure.
                {
                    SetErrorMessage("Database connectie gefaald");
                    return;
                }
                Username = Username.Replace(" ", ""); // remove whitespace between word: example Jan Smit, is now JanSmit.

                if (!TeacherUtils.AddTeacher(Username, name)) // If the teacher wasn't succesfully added.
                {
                    App.ShowMessageBox("Er is een fout opgetreden bij het aanmaken van een leerling.", "Fout", MessageBoxButton.OK);
                    return;
                }
                App.ShowMessageBox($"De leerling is aangemaakt.\n Gebruikersnaam: {Username}", "Success", MessageBoxButton.OK);
                Return(); // Return to home
            }
        }

        /// <summary>
        /// Go back to the home view.
        /// </summary>
        private void Return()
        {
            App.CurrentWindow = new AdminHomeView();
        }

        /// <summary>
        /// Changes the error message that is being displayed.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void SetErrorMessage(string message)
        {
            ErrorMessage = message;
            App.CurrentWindow.UpdateLayout();
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
