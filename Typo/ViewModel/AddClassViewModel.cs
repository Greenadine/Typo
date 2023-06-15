using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using Typo.Model;
using Typo.Model.User;
using Typo.Utils;
using Typo.View;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class AddClassViewModel : PropertyChangable, IViewModel
    {
        // Sidebar
        public AdminSidebarView Sidebar { get; set; }

        // Variables
        private string _errorMessage = string.Empty;
        private bool _teacherSort = true;

        #region Binding properties
        public TeacherModel? SelectedTeacher { get; set; }

        private ObservableCollection<TeacherModel> _teacherList;
        public ObservableCollection<TeacherModel> TeacherList
        {
            get => _teacherList;
            set { _teacherList = value; OnPropertyChanged(nameof(TeacherList)); }
        }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                App.CurrentWindow.UpdateLayout();
            }
        }
        public string Name { get; set; } = string.Empty;
        #endregion

        #region Commands
        public Command BackCommand { get; set; }
        public Command SettingsCommand { get; set; }
        public Command LogoutCommand { get; set; }
        public Command CreateClassCommand { get; set; }
        public Command TeacherSortCommand { get; set; }
        #endregion

        public AddClassViewModel()
        {
            Sidebar = new();

            _teacherList = GetTeachers();

            BackCommand = new(Back, null);
            SettingsCommand = new(Settings, null);
            LogoutCommand = new(Logout, null);
            CreateClassCommand = new(CreateClass, null);
            TeacherSortCommand = new(TeacherSort, null);
        }

        /// <summary>
        /// Sorts teachers by descending and ascending.
        /// </summary>
        public void TeacherSort()
        {
            if (_teacherSort)
            {
                var result = new ObservableCollection<TeacherModel>((from t in TeacherList orderby t.Username descending select t).ToList());
                TeacherList = result;
                _teacherSort = false;
            }
            else
            {
                var result = new ObservableCollection<TeacherModel>((from t in TeacherList orderby t.Username ascending select t).ToList());
                TeacherList = result;
                _teacherSort = true;
            }
        }

        /// <summary>
        /// Check if the input is correct for inserting into the database.
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            string name = Name.Trim();
            if (name.Equals(string.Empty))
            {
                ErrorMessage = "Vul een naam in";
                return false;
            }
            return true;
        }
        /// <summary>
        /// Create class and inserts into the database.
        /// </summary>
        private void CreateClass()
        {
            if (!CheckInput())
            {
                return;
            }
            if (ClassUtils.CheckClassName(Name) == -1)
            {
                App.ShowMessageBox("Er is iets mis gegaan met de connectie met de database.", "Fout", MessageBoxButton.OK);
                return;
            }
            else
            {
                if (ClassUtils.CheckClassName(Name) == 0)
                {
                    if (!ClassUtils.AddClass(Name)) // If the Class wasn't succesfully added.
                    {
                        App.ShowMessageBox("Er is een fout opgetreden bij het aanmaken van een Klas.", "Fout", MessageBoxButton.OK);
                        return;
                    }
                }
                else
                {
                    ErrorMessage = "De klasnaam'" + Name + "' bestaat al.";
                    return;
                }
            }

            int ClassID = ClassUtils.GetClassID(Name); // Gets the id of the class that just was created, because the ID is generated in the database.

            if (SelectedTeacher != null)    // Check so there is no can be null warning
            {
                if (!TeacherUtils.LinkTeacherToClass(SelectedTeacher.Username, ClassID)) // Links the teacher to the class in the database.
                {
                    App.ShowMessageBox("Er is een fout opgetreden bij het toevoegen van een docent aan de klas.", "Fout", MessageBoxButton.OK);
                    return;
                }

            }
            App.ShowMessageBox($"De klas is aangemaakt.", "Success!", MessageBoxButton.OK);  // Message if its succesfull

            App.CurrentWindow = new AdminHomeView(); // return to the home page
        }

        /// <summary>
        /// Retrieves all Teachers out of the database.
        /// </summary>
        private static ObservableCollection<TeacherModel> GetTeachers()
        {
            ObservableCollection<TeacherModel> teachers = new();    // Create empty collection.

            foreach (Dictionary<string, object> teacherData in TeacherUtils.GetTeachersData())  // retrieves teachers from database and loops through them individually
            {
                teachers.Add(ConstructorViewModel.ConstructTeacher(teacherData));   // Creates a new studentmodel for the student from the database.
            }

            return teachers;
        }

        /// <summary>
        /// Go back to the home view
        /// </summary>
        private static void Back()
        {
            App.CurrentWindow = new AdminHomeView();
        }

        /// <summary>
        /// Shows the setting dialog.
        /// </summary>
        private static void Settings()
        {
            // TODO implement
        }

        /// <summary>
        /// Shows the logout dialog.
        /// </summary>
        private static void Logout()
        {
            App.ShowDialog(new LogoutExitView());
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
