namespace Typo.Model.Exercise
{
    public class TextExerciseModel : ExerciseModel
    {
        #region Properties
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        #endregion

        public TextExerciseModel(int id, int classId, string name, Difficulty difficulty, bool visibility, string text) : base(id, classId, name, Mode.Text, difficulty, visibility)
        {
            _text = text;
        }
    }
}
