using System.ComponentModel;

namespace Typo.Model
{
    public abstract class PropertyChangable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T field, T value)
        {
            field = value;
            OnPropertyChanged(nameof(field));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
