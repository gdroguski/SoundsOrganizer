using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using SoundsOrganizer.Logic.PropertyHelpers;
using SoundsOrganizer.Models.Recording;

namespace SoundsOrganizer.ViewModels.Recording
{
    public class RecordingsViewModel : PropertyChangedBase
    {
        private readonly RecordingsModel _recordingsModel;

        public RecordingsViewModel(RecordingsModel recordingsModel)
        {
            _recordingsModel = recordingsModel;
            _recordingsModel.PropertyChanged += ModelPropertyChanged;

            CreateWorkingDirectory();
        }

        private void CreateWorkingDirectory()
        {
            Directory.CreateDirectory(OutputFolder);
            foreach (var file in Directory.GetFiles(OutputFolder))
            {
                Recordings.Insert(0, file);
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public string OutputFolder => _recordingsModel.RecordingsFolder;

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
    }
}
