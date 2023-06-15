namespace Typo.Model.User
{
    public class UserModel : PropertyChangable
    {
        // Variables
        private string name;

        // Properties
        public string Username { get; private set; }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        // Constructors
        public UserModel(string username, string name)
        {
            Username = username;
            this.name = name;
        }
    }
}
