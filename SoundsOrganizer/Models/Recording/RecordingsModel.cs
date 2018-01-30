using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using SoundsOrganizer.Logic.PropertyHelpers;

namespace SoundsOrganizer.Models.Recording
{
    public class RecordingsModel : PropertyChangedBase
    {
        private string _selectedRecording;
        private ObservableCollection<string> _recordings;

        public RecordingsModel()
        {
            InitializeFields();
        }

        private void InitializeFields()
        {
            _recordings = new ObservableCollection<string>();
            RecordingsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Path.GetTempPath(), @"BurpDB\Recordings");
        }

        public string SelectedRecording
        {
            get { return _selectedRecording; }
            set
            {
                if (_selectedRecording == value)
                    return;
                _selectedRecording = value;
                OnPropertyChanged(nameof(SelectedRecording));
            }
        }

        public ObservableCollection<string> Recordings
        {
            get { return _recordings; }
            set
            {
                if(_recordings == value)
                    return;
                _recordings = value;
                OnPropertyChanged(nameof(Recordings));
            }
        }

        public string RecordingsFolder { get; private set; }
    }
}
