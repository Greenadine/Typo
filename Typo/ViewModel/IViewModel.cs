namespace Typo.ViewModel
{
    public interface IViewModel
    {
        /// <summary>
        /// Updates certain properties of the viewmodel. Meant to be used after the App.CurrentWindow has been overridden.
        /// </summary>
        public void Update();
    }
}
