using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using SoundsOrganizer.Logic.Commands;
using SoundsOrganizer.Models.Rating;
using SoundsOrganizer.Models.Recording;

namespace SoundsOrganizer.ViewModels.Rating
{
    public class RatingControllerViewModel
    {
        private readonly RatingModel _ratingModel;
        private readonly RecordingsModel _recordingsModel;
        private string _selectedRecording;
        private string _operatingData;
        
        public DelegateCommand SaveCommand { get; private set; }

        public RatingControllerViewModel(RatingModel ratingModel, RecordingsModel recordingsModel)
        {
            _ratingModel = ratingModel;
            _ratingModel.PropertyChanged += ModelPropertyChanged;
            _recordingsModel = recordingsModel;
            _recordingsModel.PropertyChanged += ModelPropertyChanged;

            InitializeCommand();
            CreateWorkingDirectory();
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_recordingsModel.SelectedRecording))
            {
                _selectedRecording = _recordingsModel.SelectedRecording;
                if (_selectedRecording != null)
                {
                    SaveCommand.IsEnabled = true;
                    UpdateCurrentContextData();
                }
            }
            UpdateOperatingData();
        }

        private void UpdateCurrentContextData()
        {
            UpdateCurrentRating();
            var fileinfo = new FileInfo(CurrentRating);
            try
            {
                if (fileinfo.Exists)
                {
                    var input = File.ReadAllText(CurrentRating);
                    DisplayCurrentContextData(input);
                }
                else
                    DisplayDefaultContextData();
            }
            catch
            {
                MessageBox.Show("Could not read context data! " );
            }
        }

        private void DisplayCurrentContextData(string input)
        {
            var data = input.Split(' ');
            _ratingModel.Loudness = double.Parse(data[0]);
            _ratingModel.Length = double.Parse(data[1]);
            _ratingModel.Pitch = double.Parse(data[2]);
            _ratingModel.Humidity = double.Parse(data[3]);
            _ratingModel.BreaksCount = int.Parse(data[4]);
            _ratingModel.Burpage = double.Parse(data[5]);
        }

        private void DisplayDefaultContextData()
        {
            _ratingModel.Loudness =
                _ratingModel.Length = _ratingModel.Pitch = _ratingModel.Humidity = _ratingModel.Burpage = 0.5;
            _ratingModel.BreaksCount = 0;
        }

        private void UpdateOperatingData()
        {
            _operatingData =
                $"{_ratingModel.Loudness:0.#00} {_ratingModel.Length:0.#00} {_ratingModel.Pitch:0.#00} {_ratingModel.Humidity:0.#00} {_ratingModel.BreaksCount} {_ratingModel.Burpage:0.#00}";
        }

        private void InitializeCommand()
        {
            SaveCommand = new DelegateCommand(Save) {IsEnabled = false};
        }

        private void Save()
        {
            try
            {
                UpdateCurrentRating();
                File.WriteAllText(CurrentRating, _operatingData);
                MessageBox.Show("Saved!");
            }
            catch
            {
                MessageBox.Show("Could not save context data!");
            }
        }

        private void UpdateCurrentRating()
        {
            var burpIndex = _selectedRecording.LastIndexOf("Burp", StringComparison.Ordinal);
            CurrentRating =
                $@"{RatingFolder}\{_selectedRecording.Substring(burpIndex, _selectedRecording.Length - burpIndex - 4)}.txt";
        }

        private void CreateWorkingDirectory()
        {
            Directory.CreateDirectory(RatingFolder);
        }

        public string CurrentRating
        {
            get { return _ratingModel.CurrentRating; }
            set { _ratingModel.CurrentRating = value; }
        }

        public string RatingFolder => _ratingModel.RatingsFolder;
    }
}
