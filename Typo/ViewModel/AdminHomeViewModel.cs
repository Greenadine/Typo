using Typo.View;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class AdminHomeViewModel : IViewModel
    {
        // Sidebar
        public AdminSidebarView Sidebar { get; set; }

        #region Commands
        public Command AddAccountCommand { get; set; }
        public Command AddClassCommand { get; set; }
        public Command LinkStudentToClassCommand { get; set; }
        public Command LinkTeacherToClassCommand { get; set; }
        #endregion

        public AdminHomeViewModel()
        {
            Sidebar = new();

            AddAccountCommand = new(AddAccount);
            AddClassCommand = new(AddClass);
            LinkStudentToClassCommand = new(LinkStudentToClass);
            LinkTeacherToClassCommand = new(LinkTeacherToClass);
        }

        /// <summary>
        /// Navigates to the view for addings accounts.
        /// </summary>
        private void AddAccount()
        {
            App.CurrentWindow = new AddAccountView();
        }

        /// <summary>
        /// Navigates to the view for adding classes.
        /// </summary>
        private void AddClass()
        {    
           App.CurrentWindow = new AddClassView();
        }

        /// <summary>
        /// Navigates to the view for linking students to classes.
        /// </summary>
        private void LinkStudentToClass()
        {
            App.CurrentWindow = new LinkStudentToClassView();
        }

        /// <summary>
        /// Navigates to the view for linking teachers to classes.
        /// </summary>
        private void LinkTeacherToClass()
        {
            App.CurrentWindow = new LinkTeacherToClassView();
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
