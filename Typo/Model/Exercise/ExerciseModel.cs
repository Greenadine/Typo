namespace Typo.Model.Exercise
{
    public abstract class ExerciseModel : PropertyChangable
    {
        #region Properties
        public int Id { get; }
        private int _classId;
        public int ClassId
        {
            get => _classId;
            set
            {
                _classId = value;
                OnPropertyChanged(nameof(ClassId));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private Mode _mode;
        public Mode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        private Difficulty _difficulty;
        public Difficulty Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        private bool _visibility;
        public bool Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
        #endregion

        public ExerciseModel(int id, int classId, string name, Mode mode, Difficulty difficulty, bool visibility)
        {
            Id = id;
            _classId = classId;
            _name = name;
            _mode = mode;
            _difficulty = difficulty;
            _visibility = visibility;
        }
    }
}
