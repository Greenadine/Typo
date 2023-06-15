using Typo.View;

namespace Typo.ViewModel
{
    public class MarathonResultsViewModel : ResultViewModel
    {
        public int? WordsDone => Result.WordsDone;

        public override Command AgainCommand { get; set; }

        public MarathonResultsViewModel() : base(MarathonExerciseViewModel.Result)
        {
            AgainCommand = new(RetryExercise, null);
        }

        /// <summary>
        /// Command function to retry the exercise after a button press
        /// </summary>
        public void RetryExercise()
        {
            App.CurrentWindow = new ExecuteMarathonExerciseView();
        }
    }
}