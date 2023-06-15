using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Typo.Model;
using Typo.Model.User;
using Typo.Utils;
using Typo.View;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class LinkTeacherToClassViewModel : PropertyChangable, IViewModel
    {
        // Sidebar
        public AdminSidebarView Sidebar { get; set; }

        // Variables
        private string _errorMessage = string.Empty;
        private bool _teacherSort = true;
        private bool _classSort = true;

        // Properties
        private List<ClassModel>? _classList;
        public List<ClassModel>? ClassList
        {
            get => _classList;
            set { _classList = value; OnPropertyChanged(nameof(ClassList)); }
        }
        private List<TeacherModel>? _teacherList;
        public List<TeacherModel>? TeacherList
        {
            get => _teacherList;
            set { _teacherList = value; OnPropertyChanged(nameof(TeacherList)); }
        }
        public TeacherModel? SelectedTeacher { get; set; }
        public ClassModel? SelectedClass { get; set; }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // Commands
        public Command LinkTeacherCommand { get; set; }
        public Command BackCommand { get; set; }
        public Command SettingsCommand { get; set; }
        public Command LogoutCommand { get; set; }
        public Command TeacherSortCommand { get; set; }
        public Command ClassSortCommand { get; set; }

        public LinkTeacherToClassViewModel()
        {
            Sidebar = new();

            ClassList = GetClasses();
            TeacherList = GetTeachers();
            SelectedClass = null;
            SelectedTeacher = null;

            LogoutCommand = new(Logout, null);
            SettingsCommand = new(Settings, null);
            BackCommand = new(Back, null);
            LinkTeacherCommand = new(LinkTeacher, null);
            ClassSortCommand = new(ClassSort, null);
            TeacherSortCommand = new(TeacherSort, null);
        }

        /// <summary>
        /// Sorts teachers by descending and ascending.
        /// </summary>
        public void TeacherSort()
        {
            if (_teacherSort)
            {
                var result = (from t in TeacherList orderby t.Username descending select t).ToList();
                TeacherList = result;
                _teacherSort = false;
            }
            else
            {
                var result = (from t in TeacherList orderby t.Username ascending select t).ToList();
                TeacherList = result;
                _teacherSort = true;
            }
        }

        /// <summary>
        /// Sorts Classes by descending and ascending.
        /// </summary>
        public void ClassSort()
        {
            if (_classSort)
            {
                var result = (from s in ClassList orderby s.Name descending select s).ToList();
                ClassList = result;
                _classSort = false;
            }
            else
            {
                var result = (from s in ClassList orderby s.Name ascending select s).ToList();
                ClassList = result;
                _classSort = true;
            }
        }

        /// <summary>
        /// Retrieves all Teachers out of the database.
        /// </summary>
        private List<TeacherModel> GetTeachers()
        {
            List<TeacherModel> teachers = new();    // Create empty list.

            foreach (Dictionary<string, object> teacherData in TeacherUtils.GetTeachersData())  // retrieves teachers from database and loops through them individually
            {
                teachers.Add(ConstructorViewModel.ConstructTeacher(teacherData));   // Creates a new studentmodel for the student from the database.
            }

            return teachers;
        }
        /// <summary>
        /// Retrieves all classes out of the database.
        /// </summary>
        private List<ClassModel> GetClasses()
        {
            List<ClassModel> classes = new();   // Create empty list.

            foreach (Dictionary<string, object> ClassData in ClassUtils.GetClassesData())    // retrieves Classes from database and loops through them individually.
            {
                classes.Add(ConstructorViewModel.ConstructClass(ClassData));    // Creates a new ClassModel for the Class from the database.
            }
            return classes;
        }

        /// <summary>
        /// Go back to the home view
        /// </summary>
        private void Back()
        {
            App.CurrentWindow = new AdminHomeView();
        }
        /// <summary>
        /// Shows the setting dialog.
        /// </summary>
        private void Settings()
        {
            // TODO implement
        }

        /// <summary>
        /// Shows the logout dialog.
        /// </summary>
        private void Logout()
        {
            App.ShowDialog(new LogoutExitView());
        }
        /// <summary>
        /// Checks if there are items selected in the listviews.
        /// </summary>
        private bool ItemsSelected()
        {
            return SelectedClass != null && SelectedTeacher != null;
        }
        /// <summary>
        /// Links the Selected teacher(s) to the selected class.
        /// </summary>
        private void LinkTeacher()
        {
            if (!ItemsSelected())
            {
                ErrorMessage = "Selecteer een klas en docent";  // error message.
                return;
            }

            if (SelectedClass == null || SelectedTeacher == null)
            {
                return;
            }

            if (ClassUtils.CheckTeacherClassLink(SelectedTeacher.Username, SelectedClass.Id) == 0)
            {
                if (!TeacherUtils.LinkTeacherToClass(SelectedTeacher.Username, SelectedClass.Id))    // Links the teacher to the class in the database.
                {
                    App.ShowMessageBox("Er is een fout opgetreden bij het toevoegen van een docent aan de klas.", "Fout", MessageBoxButton.OK);
                    return;
                }
                App.ShowMessageBox("De docent is gekoppled aan de klas", "Success", MessageBoxButton.OK);

                App.CurrentWindow = new AdminHomeView(); // Return to home.
                return;
            }

            App.ShowMessageBox("De geselecteerde docent is al aan deze klas gekoppeld", "Fout", MessageBoxButton.OK);
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
