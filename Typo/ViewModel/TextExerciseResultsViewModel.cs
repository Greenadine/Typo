using DiffPlex;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Typo.Model.Exercise;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public class TextExerciseResultsViewModel : ResultViewModel
    {
        public DiffPaneModel DiffModel { get; set; }
        private TextExerciseModel _exerciseModel;

        public override Command AgainCommand { get; set; }

        public TextExerciseResultsViewModel() : base(TextExerciseViewModel.Result)
        {
            _exerciseModel = (TextExerciseModel)ConstructorViewModel.ConstructExercise(ExerciseUtils.GetExerciseById(Result.ExerciseId));
            BackCommand = new(() => App.CurrentWindow = new StudentHomeView());
            AgainCommand = new(() => App.CurrentWindow = new ExecuteTextExerciseView());

            var diffBuilder = new InlineDiffBuilder(new Differ());
            DiffModel = diffBuilder.BuildDiffModel(_exerciseModel.Text, TextExerciseViewModel.Input, true, false, new WordChunker());
        }

        /// <summary>
        /// prepares the time spent on the exercise into a readable string to assign to to  the view
        /// </summary>
        /// <param name="TimeInSeconds"></param>
        /// <returns></returns>
        public static string TimerToReadableString(int TimeInSeconds)
        {
            int minutes = TimeInSeconds / 60; // aantal seconden delen door 60 geeft het aantal minuten
            int seconds = TimeInSeconds % 60; // aantal seconden modulo 60 geeft het aantal seconden - alle seconden die binnen een minuut vallen

            string ReadableTimer;

            // Minutes
            if (minutes < 10)
            {
                ReadableTimer = '0' + minutes.ToString();
            }
            else
            {
                ReadableTimer = minutes.ToString();
            }

            // Seconds
            if (seconds < 10)
            {
                ReadableTimer += ":0" + seconds;
            }
            else
            {
                ReadableTimer += ":" + seconds;
            }
            return ReadableTimer;
        }
    }
}
