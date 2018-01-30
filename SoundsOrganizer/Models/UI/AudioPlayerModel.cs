using NAudio.Wave;
using SoundsOrganizer.Logic.PropertyHelpers;

namespace SoundsOrganizer.Models.UI
{
    public class AudioPlayerModel : PropertyChangedBase
    {
        private WaveOut _playbackDevice;
        private bool _isPlaying;
        private bool _isPaused;

        public AudioPlayerModel()
        {
            CreateDevice();
            _isPlaying = _playbackDevice.PlaybackState == PlaybackState.Playing;
            _isPaused = _playbackDevice.PlaybackState == PlaybackState.Paused;
        }

        public void CreateDevice()
        {
            _playbackDevice = new WaveOut { DesiredLatency = 200 };
        }

        public WaveOut PlaybackDevice
        {
            get
            {
                return _playbackDevice;
            }

            set
            {
                if (_playbackDevice == value)
                    return;
                _playbackDevice = value;
                OnPropertyChanged(nameof(PlaybackDevice));
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }

            set
            {
                if (_isPlaying == value)
                    return;
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        public bool IsPaused
        {
            get
            {
                return _isPaused;
            }

            set
            {
                if (_isPaused == value)
                    return;
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }
    }
}
