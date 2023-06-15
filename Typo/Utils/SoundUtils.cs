using System.Windows.Media;

namespace Typo.Utils
{
    public class SoundUtils
    {
        private static MediaPlayer mediaPlayer = new();
        public static double Volume { get; private set; } = 5;

        /// <summary>
        /// Plays the sound at the given path once.
        /// </summary>
        /// <param name="path">The path to the sound file to play.</param>
        public static void PlayOnce(string path)
        {
            mediaPlayer.Open(new System.Uri(path));
            mediaPlayer.Play();
        }

        /// <summary>
        /// Sets the volume from the variable and updates the value of the display of the slider.
        /// </summary>
        /// <param name="volume"></param>
        public static void SetVolume(double volume)
        {
            mediaPlayer.Volume = volume;
            Volume = volume * 10;
        }
    }
}
