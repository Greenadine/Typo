using System.Windows;

namespace Typo.View
{
    /// <summary>
    /// Interaction logic for ExecuteWordsExerciseView.xaml
    /// </summary>
    public partial class ExecuteWordsExerciseView : Window
    {
        public ExecuteWordsExerciseView()
        {
            InitializeComponent();
        }

        private void TextBox_Input_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_Input.Focus();
        }
    }
}