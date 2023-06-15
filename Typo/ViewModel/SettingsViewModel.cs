using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typo.Model;
using Typo.Utils;

namespace Typo.ViewModel
{
    public class SettingsViewModel : PropertyChangable
    {
        #region Binding properties
        public double SliderValue { 
            get 
            { 
                return SoundUtils.Volume; 
            } 
            set 
            { 
                SoundUtils.SetVolume(value/10);
                SetVolumeValue(value);
                string correctSound = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Sounds", "CorrectSoundTypo.wav"); // Creates a relative path in a string
                SoundUtils.PlayOnce(correctSound);  // plays the sound
            }  
        }

        private double _volumeValue; 
        public double VolumeValue
        {
            get { return _volumeValue; }
            set
            {
                _volumeValue = Math.Floor(SliderValue);   
                OnPropertyChanged(nameof(VolumeValue));
            }
        }
        #endregion

        public Command ExitCommand { get;  set; }

        public SettingsViewModel()
        {
            SetVolumeValue(VolumeValue);
            ExitCommand = new(Exit);
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        private void Exit()
        {
            App.CurrentDialog = null;
        }

        /// <summary>
        /// Sets the value of the volume so it will display on the screen correctly.
        /// </summary>
        /// <param name="volume">The new volume.</param>
        private void SetVolumeValue(double volume)
        {
            VolumeValue = volume;
            App.CurrentWindow.UpdateLayout();
        }
    }
}
