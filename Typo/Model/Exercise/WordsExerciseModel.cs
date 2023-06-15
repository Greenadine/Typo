using System.Collections.Generic;

namespace Typo.Model.Exercise
{
    public class WordsExerciseModel : ExerciseModel
    {
        #region Properties
        private List<string> _words;
        public List<string> Words
        {
            get => _words;
            set
            {
                _words = value;
                OnPropertyChanged(nameof(Words));
            }
        }
        #endregion

        public WordsExerciseModel(int id, int classId, string? name, Difficulty difficulty, bool visibility, List<string> words) : base(id, classId, name, Mode.Words, difficulty, visibility)
        {
            _words = words;
        }
    }
}
