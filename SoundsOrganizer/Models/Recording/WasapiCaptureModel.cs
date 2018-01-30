using System.Collections.Generic;
using NAudio.CoreAudioApi;
using SoundsOrganizer.Logic.PropertyHelpers;

namespace SoundsOrganizer.Models.Recording
{
    public class WasapiCaptureModel : PropertyChangedBase
    {
        private float _peak;
        private MMDevice _selectedDevice;
        private int _sampleRate;
        private int _bitDepth;
        private int _channelCount;
        private int _sampleTypeIndex;
        private string _message;
        private int _shareModeIndex;
        private float _recordLevel;
        private bool _isAnyDevice;
        private IEnumerable<MMDevice> _captureDevices;

        public float Peak
        {
            get { return _peak; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_peak == value)
                    return;
                _peak = value;
                OnPropertyChanged(nameof(Peak));
            }
        }

        public MMDevice SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                if (_selectedDevice == value)
                    return;
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }

        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                if (_sampleRate == value)
                    return;
                _sampleRate = value;
                OnPropertyChanged(nameof(SampleRate));
            }
        }

        public int BitDepth
        {
            get
            {
                return _bitDepth;
            }
            set
            {
                if (_bitDepth == value)
                    return;
                _bitDepth = value;
                OnPropertyChanged(nameof(BitDepth));
            }
        }

        public int ChannelCount
        {
            get
            {
                return _channelCount;
            }
            set
            {
                if (_channelCount == value)
                    return;
                _channelCount = value;
                OnPropertyChanged(nameof(ChannelCount));
            }
        }

        public bool IsBitDepthConfigurable => SampleTypeIndex == 1;

        public int SampleTypeIndex
        {
            get
            {
                return _sampleTypeIndex;
            }
            set
            {
                if (_sampleTypeIndex == value)
                    return;
                _sampleTypeIndex = value;
                OnPropertyChanged(nameof(SampleTypeIndex));
                BitDepth = _sampleTypeIndex == 1 ? 16 : 32;
                OnPropertyChanged(nameof(IsBitDepthConfigurable));
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message == value)
                    return;
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public int ShareModeIndex
        {
            get { return _shareModeIndex; }
            set
            {
                if (_shareModeIndex == value)
                    return;
                _shareModeIndex = value;
                OnPropertyChanged(nameof(ShareModeIndex));
            }
        }

        public float RecordLevel
        {
            get { return _recordLevel; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_recordLevel == value)
                    return;
                _recordLevel = value;
                OnPropertyChanged(nameof(RecordLevel));
            }
        }

        public bool IsAnyDevice
        {
            get { return _isAnyDevice; }
            set
            {
                if (_isAnyDevice == value)
                    return;
                _isAnyDevice = value;
                OnPropertyChanged(nameof(IsAnyDevice));
            }
        }

        public IEnumerable<MMDevice> CaptureDevices
        {
            get { return _captureDevices; }
            set
            {
                if (Equals(_captureDevices, value))
                    return;
                _captureDevices = value;
                OnPropertyChanged(nameof(CaptureDevices));
            }
        }
    }
}
