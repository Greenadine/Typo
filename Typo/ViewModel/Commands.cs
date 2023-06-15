using System.Windows;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    /// <summary>
    /// Class containing commonly-used commands.
    /// </summary>
    public class Commands
    {
        #region Main
        // TODO implement settings
        public static readonly Command SETTINGS = new(() =>
        {
            App.CurrentDialog = new SettingsView();
        });
        public static readonly Command LOGOUT_EXIT = new(() => App.CurrentDialog = new LogoutExitView());
        public static readonly Command EXIT = new(() => Application.Current.Shutdown());
        #endregion

        #region Student
        public static readonly Command STUDENT_HOME = new(() => App.CurrentWindow = new StudentHomeView());
        public static readonly Command STUDENT_EXERCISES = new(() => App.CurrentWindow = new StudentExercisesView());
        public static readonly Command STUDENT_STATISTICS = new(() => App.CurrentWindow = new StudentStatisticsView());
        public static readonly Command STUDENT_MILESTONES = new(() => App.CurrentWindow = new StudentMilestonesView());

        #endregion

        #region Teacher
        public static readonly Command TEACHER_HOME = new(() => App.CurrentWindow = new TeacherHomeView());
        #endregion

        #region Admin
        // TODO implement admin home view
        public static readonly Command ADMIN_HOME = new(() =>
        {
            // App.CurrentWindow = new AdminHomeView()
            UserUtils.Logout();
            App.CurrentWindow = new LoginView();
        });
        #endregion
    }
}
