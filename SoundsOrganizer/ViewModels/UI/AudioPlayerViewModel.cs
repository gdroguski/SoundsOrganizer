using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using SoundsOrganizer.Logic.Audio.Player;
using SoundsOrganizer.Logic.Audio.Sampling;
using SoundsOrganizer.Logic.Commands;
using SoundsOrganizer.Logic.PropertyHelpers;
using SoundsOrganizer.Models.Rating;
using SoundsOrganizer.Models.Recording;
using SoundsOrganizer.Models.UI;
using SoundsOrganizer.ViewModels.Presentation;

namespace SoundsOrganizer.ViewModels.UI
{
    public class AudioPlayerViewModel : PropertyChangedBase
    {
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

        private readonly VisualisationViewModel _visualization;
        private readonly AudioPlayerHandler _audioPlayerHandler;
        private readonly RecordingsModel _recordingsModel;
        private readonly RatingModel _ratingModel;
        private readonly AudioPlayerModel _audioPlayerModel;

        public AudioPlayerViewModel(VisualisationViewModel visualisationViewModel, RecordingsModel recordingsModel, AudioPlayerModel audioPlayerModel, RatingModel ratingModel)
        {
            _visualization = visualisationViewModel;
            _recordingsModel = recordingsModel;
            _recordingsModel.PropertyChanged += ModelPropertyChanged;
            _ratingModel = ratingModel;
            _audioPlayerModel = audioPlayerModel;
            _audioPlayerModel.PropertyChanged += ModelPropertyChanged;

            _audioPlayerHandler = new AudioPlayerHandler(_audioPlayerModel);
            _audioPlayerHandler.MaximumCalculated += audioGraph_MaximumCalculated;
            InitializeCommands();
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            UpdateCommands();
        }

        private void audioGraph_MaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            _visualization.AddValue(e.MinSample, e.MaxSample);
        }

        private void InitializeCommands()
        {
            OpenCommand = new DelegateCommand(OpenAndPlayFile);
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            StopCommand = new DelegateCommand(Stop);
            ClearCommand = new DelegateCommand(Clear);
            DeleteCommand = new DelegateCommand(Delete);
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            PlayCommand.IsEnabled = (SelectedRecording != null && !IsPlaying) || (SelectedRecording != null && IsPaused);
            DeleteCommand.IsEnabled = SelectedRecording != null && !IsPlaying && !IsPaused;
            PauseCommand.IsEnabled = IsPlaying;
            StopCommand.IsEnabled = IsPlaying;
        }

        private void Play()
        {
            if (SelectedRecording == null)
                OpenAndPlayFile();
            if (!IsPaused)
            {
                _audioPlayerHandler.Load(SelectedRecording);
                Clear();
            }
            _audioPlayerHandler.Play();
        }

        private void Pause()
        {
            _audioPlayerHandler.Pause();
        }

        private void Stop()
        {
            _audioPlayerHandler.Stop();
            Clear();
        }

        private void Clear()
        {
            _visualization.Reset();
        }

        private void Delete()
        {
            try
            {
                File.Delete(Path.Combine(RecordingsFolder, SelectedRecording));
                var currentRating = Path.Combine(RatingsFolder, _ratingModel.CurrentRating);
                if (File.Exists(currentRating))
                    File.Delete(currentRating);
                Recordings.Remove(SelectedRecording);
                SelectedRecording = Recordings.FirstOrDefault();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not delete recording");
            }
        }

        private void OpenAndPlayFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All Supported Files (*.wav)|*.wav"
            };
            var result = openFileDialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            _audioPlayerHandler.Load(openFileDialog.FileName);
            Clear();
            _audioPlayerHandler.Play();
        }

        public string RecordingsFolder => _recordingsModel.RecordingsFolder;

        public string RatingsFolder => _ratingModel.RatingsFolder;

        public string SelectedRecording
        {
            get { return _recordingsModel.SelectedRecording; }
            set { _recordingsModel.SelectedRecording = value; }
        }

        public ObservableCollection<string> Recordings
        {
            get { return _recordingsModel.Recordings; }
            set { _recordingsModel.Recordings = value; }
        }

        public bool IsPlaying => _audioPlayerModel.IsPlaying;

        public bool IsPaused => _audioPlayerModel.IsPaused;
    }
}
