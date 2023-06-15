using System.Windows;

namespace Typo.View
{
    /// <summary>
    /// Interaction logic for ExecuteMarathonExerciseView.xaml
    /// </summary>
    public partial class ExecuteMarathonExerciseView : Window
    {
        public ExecuteMarathonExerciseView()
        {
            InitializeComponent();
        }

        private void TextBox_Input_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_Input.Focus();
        }
    }
}