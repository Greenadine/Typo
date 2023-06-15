using Typo.Model;

namespace Typo.ViewModel
{
    public class HelpViewModel
    {
        public static WindowHelpModel? Help { get; set; }

        public Command CloseCommand { get; } = new(Close);

        public string HelpTitle { get => Help != null ? Help.Title : string.Empty; }
        public string HelpDescription { get => Help != null ? Help.Description : string.Empty; }

        private static void Close()
        {
            App.CurrentDialog?.Close();
        }
    }
}
