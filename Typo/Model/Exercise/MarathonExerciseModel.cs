using System.Collections.Generic;

namespace Typo.Model.Exercise
{
    public class MarathonExerciseModel : ExerciseModel
    {
        #region Properties
        private List<string> _words;
        public List<string> Words
        {
            get => _words;
            set
            {
                _words = value;
                OnPropertyChanged("Marathon");
            }
        }
        #endregion

        public MarathonExerciseModel(int id, int classId, string name, Difficulty difficulty, bool visibility, List<string> words) : base(id, classId, name, Mode.Marathon, difficulty, visibility)
        {
            _words = words;
        }
    }
}