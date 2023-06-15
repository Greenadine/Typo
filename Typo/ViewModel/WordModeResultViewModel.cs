using Typo.View;

namespace Typo.ViewModel
{
    public class WordModeResultViewModel : ResultViewModel
    {
        public override Command AgainCommand { get; set; }

        public WordModeResultViewModel() : base(WordExerciseViewModel.Result)
        {
            AgainCommand = new Command(RetryExercise, null);
        }

        /// <summary>
        /// Command function to retry the exercise after a button press
        /// </summary>
        public void RetryExercise()
        {
            App.CurrentWindow = new ExecuteWordsExerciseView();
        }
    }
}
