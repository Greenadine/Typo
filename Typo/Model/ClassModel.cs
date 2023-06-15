namespace Typo.Model
{
    public class ClassModel : PropertyChangable
    {
        // Variables
        private string _name;

        // Properties
        public int Id { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        // Constructors
        public ClassModel(int id, string name)
        {
            Id = id;
            _name = name;
        }
    }
}
