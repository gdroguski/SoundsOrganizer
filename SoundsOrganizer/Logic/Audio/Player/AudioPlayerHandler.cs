using System;
using System.ComponentModel;
using System.Windows;
using NAudio.Wave;
using SoundsOrganizer.Logic.Audio.Sampling;
using SoundsOrganizer.Logic.PropertyHelpers;
using SoundsOrganizer.Models.UI;

namespace SoundsOrganizer.Logic.Audio.Player
{
    internal class AudioPlayerHandler : PropertyChangedBase, IDisposable
    {
        private readonly AudioPlayerModel _audioPlayerModel;
        private WaveStream _fileStream;

        private WaveOut PlaybackDevice
        {
            get { return _audioPlayerModel.PlaybackDevice; }
            set { _audioPlayerModel.PlaybackDevice = value; }
        }

        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;

        public AudioPlayerHandler(AudioPlayerModel audioPlayerModel)
        {
            _audioPlayerModel = audioPlayerModel;
            _audioPlayerModel.PropertyChanged += ModelPropertyChanged;
            _audioPlayerModel.PlaybackDevice.PlaybackStopped += PlaybackDeviceOnPlaybackStopped;
        }
         
        private void PlaybackDeviceOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            CloseFile();
            UpdateStates();
        }

        private void UpdateStates()
        {
            _audioPlayerModel.IsPlaying = PlaybackDevice.PlaybackState == PlaybackState.Playing;
            _audioPlayerModel.IsPaused = PlaybackDevice.PlaybackState == PlaybackState.Paused;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public void Load(string fileName)
        {
            Stop();
            CloseFile();
            EnsureDeviceCreated();
            OpenFile(fileName);
            UpdateStates();
        }

        public void Play()
        {
            if (PlaybackDevice == null || _fileStream == null)
                return;
            PlaybackDevice.Play();
            UpdateStates();
        }

        public void Pause()
        {
            if(PlaybackDevice.PlaybackState == PlaybackState.Playing)
                PlaybackDevice.Pause();
            UpdateStates();
        }

        public void Stop()
        {
            PlaybackDevice.Stop();
            if (_fileStream != null)
            {
                _fileStream.Position = 0;
            }
            UpdateStates();
        }

        public void Dispose()
        {
            Stop();
            CloseFile();
            if (PlaybackDevice == null)
                return;
            PlaybackDevice.Dispose();
            PlaybackDevice = null;
        }

        protected virtual void OnMaximumCalculated(MaxSampleEventArgs e)
        {
            var handler = MaximumCalculated;
            handler?.Invoke(this, e);
        }

        private void OpenFile(string fileName)
        {
            try
            {
                var inputStream = new AudioFileReader(fileName);
                _fileStream = inputStream;

                var aggregator = new SampleAggregator(inputStream)
                {
                    NotificationCount = inputStream.WaveFormat.SampleRate/100,
                    PerformFFT = true
                };
                aggregator.MaximumCalculated += (s, a) => OnMaximumCalculated(a);
                PlaybackDevice.Init(aggregator);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Problem opening file");
                CloseFile();
            }
        }

        private void CloseFile()
        {
            if (_fileStream == null)
                return;
            _fileStream.Dispose();
            _fileStream = null;
        }

        private void EnsureDeviceCreated()
        {
            if (PlaybackDevice == null)
                CreateDevice();
        }

        private void CreateDevice()
        {
            PlaybackDevice = new WaveOut { DesiredLatency = 200 };
        }        
    }
}
