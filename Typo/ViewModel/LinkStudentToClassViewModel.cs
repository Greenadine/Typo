using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Typo.Model;
using Typo.Model.User;
using Typo.Utils;
using Typo.View;
using System.Linq;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class LinkStudentToClassViewModel : PropertyChangable, IViewModel
    {
        // Sidebar
        public AdminSidebarView Sidebar { get; set; }

        // Variables
        private string _errorMessage = string.Empty;
        private bool _studentSort = true;
        private bool _classSort = true;

        // Properties
        private List<ClassModel>? _classList;
        public List<ClassModel>? ClassList
        {
            get => _classList;
            set { _classList = value; OnPropertyChanged(nameof(ClassList)); }
        }
        private List<StudentModel>? _studentList;
        public List<StudentModel>? StudentList
        {
            get => _studentList;
            set { _studentList = value; OnPropertyChanged(nameof(StudentList)); }
        }
        public StudentModel? SelectedStudent {  get; set; }
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
        public Command LinkStudentCommand { get; set;}
        public Command BackCommand { get; set;}
        public Command SettingsCommand { get; set;}
        public Command LogoutCommand { get; set; }
        public Command StudentSortCommand { get; set;}
        public Command ClassSortCommand { get; set;}

        public LinkStudentToClassViewModel()
        {
            Sidebar = new();

            ClassList = GetClasses();
            StudentList = GetStudents();
            SelectedClass = null;
            SelectedStudent = null;

            LogoutCommand = new(Logout, null);
            SettingsCommand = new(Settings, null);
            BackCommand = new(Back, null);
            LinkStudentCommand = new(LinkStudent, null);
            StudentSortCommand = new(StudentSort, null);
            ClassSortCommand = new(ClassSort, null);
        }

        /// <summary>
        /// Sorts Students by descending and ascending.
        /// </summary>
        public void StudentSort()
        {
            if (_studentSort)
            {
                var result = (from s in StudentList orderby s.Username descending select s).ToList();
                StudentList = result;
                _studentSort = false;
            }
            else
            {
                var result = (from s in StudentList orderby s.Username ascending select s).ToList();
                StudentList = result;
                _studentSort = true;
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
            /// Retrieves all students with no class out of the database.
            /// </summary>
            private List<StudentModel> GetStudents()
        {
            List<StudentModel> students = new();    // Create empty list.

            foreach (Dictionary<string, object> studentData in StudentUtils.GetClasslessStudentsData()) // retrieves students from database and loops through them individually
            {
                students.Add(ConstructorViewModel.ConstructStudent(studentData));   // Creates a new studentmodel for the student from the database.
            }

            return students;
        }
    
        /// <summary>
        /// Retrieves all classes out of the database.
        /// </summary>
        /// <returns></returns>
        private List<ClassModel> GetClasses()   
        {
            List<ClassModel> classes = new();   // Create empty list.

            foreach(Dictionary<string, object> ClassData in ClassUtils.GetClassesData())    // retrieves Classes from database and loops through them individually.
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


        private void LinkStudent()
        {
            if (SelectedStudent != null && SelectedClass != null)   // Check so there is no can be null warning. 
            {
                if (!StudentUtils.UpdateStudentData(SelectedStudent.Username, SelectedStudent.Name, SelectedClass.Id))   // Links the students to the class in the database.
                {
                    App.ShowMessageBox("Er is een fout opgetreden bij het koppelen van een leerling aan de klas.", "Fout", MessageBoxButton.OK);
                    return;
                }
                App.ShowMessageBox($"De leerling is gekoppled aan de klas", "Success", MessageBoxButton.OK);
            }
            else
            {
                ErrorMessage = "Selecteer een klas en leerling";    // Error message.
                return;
            }
            App.CurrentWindow = new AdminHomeView();    // Return to home.
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
