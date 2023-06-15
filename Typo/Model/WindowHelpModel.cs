namespace Typo.Model
{
    public class WindowHelpModel
    {
        public string Title { get; private set; }
        public string Description { get; private set; }

        public WindowHelpModel(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
