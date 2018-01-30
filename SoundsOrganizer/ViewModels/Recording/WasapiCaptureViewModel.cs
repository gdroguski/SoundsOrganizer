using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SoundsOrganizer.Logic.Audio.Recording;
using SoundsOrganizer.Logic.Commands;
using SoundsOrganizer.Logic.PropertyHelpers;
using SoundsOrganizer.Models.Recording;
using StoppedEventArgs = NAudio.Wave.StoppedEventArgs;

namespace SoundsOrganizer.ViewModels.Recording
{
    public class WasapiCaptureViewModel : PropertyChangedBase
    {  
        public DelegateCommand RecordCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }

        private string _currentFileName;
        private WasapiCapture _capture;
        private WaveFileWriter _writer;
        private readonly SynchronizationContext _synchronizationContext;
        private readonly WasapiCaptureModel _wasapiCaptureModel;
        private readonly RecordingsModel _recordingsModel;
        private UsbDeviceDetector _deviceDetector;

        public WasapiCaptureViewModel(WasapiCaptureModel wasapiCaptureModel,RecordingsModel recordingsModel, UsbDeviceDetector deviceDetector)
        {
            _recordingsModel = recordingsModel;
            _wasapiCaptureModel = wasapiCaptureModel;
            _wasapiCaptureModel.PropertyChanged += ModelPropertyChanged;
            _synchronizationContext = SynchronizationContext.Current;

            TryInitializeDevices();
            InitializeCommands();
            DisableRecordingPossibility(SelectedDevice != null);
            InitializeUsbDetector(deviceDetector);
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName != nameof(SampleTypeIndex))
                return;
            BitDepth = SampleTypeIndex == 1 ? 16 : 32;
            OnPropertyChanged(nameof(IsBitDepthConfigurable));
        }

        private void TryInitializeDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            IsAnyDevice = CaptureDevices.Any();
            var defaultDevice = TryGetDevice(enumerator);
            SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice?.ID);
        }


        private MMDevice TryGetDevice(MMDeviceEnumerator enumerator)
        {
            try
            {
                Message = "";
                return enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            }
            catch (Exception)
            {
                Message = "No audiorecorder";
                return null;
            }
        }

        private void InitializeCommands()
        {
            RecordCommand = new DelegateCommand(Record);
            StopCommand = new DelegateCommand(Stop) { IsEnabled = false };
        }

        private void Record()
        {
            try
            {
                _capture = new WasapiCapture(SelectedDevice)
                {
                    ShareMode = ShareModeIndex == 0 ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive,
                    WaveFormat = SampleTypeIndex == 0
                        ? WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, ChannelCount)
                        : new WaveFormat(SampleRate, BitDepth, ChannelCount)
                };
                _currentFileName = $"Burp_N0_{DateTime.Now:yyy_dd_MM_HH_mm_ss}.wav";
                RecordLevel = SelectedDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                _capture.StartRecording();
                _capture.DataAvailable += CaptureOnDataAvailable;
                _capture.RecordingStopped += OnRecordingStopped;
                RecordCommand.IsEnabled = false;
                StopCommand.IsEnabled = true;
                Message = "Recording...";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CaptureOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            if (_writer == null)
            {
                _writer = new WaveFileWriter(Path.Combine(_recordingsModel.RecordingsFolder,
                        _currentFileName),
                    _capture.WaveFormat);
            }

            _writer.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);

            UpdatePeakMeter();
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            _writer.Dispose();
            _writer = null;
            var recordingPath = Path.Combine(_recordingsModel.RecordingsFolder, _currentFileName);
            _recordingsModel.Recordings.Insert(0, recordingPath);

            if (e.Exception == null)
                Message = "Recording Stopped";
            else
                Message = "Recording Error: " + e.Exception.Message;
            _capture.Dispose();
            _capture = null;
            RecordCommand.IsEnabled = true;
            StopCommand.IsEnabled = false;
            _recordingsModel.SelectedRecording = recordingPath;
        }

        private void UpdatePeakMeter()
        {
            // can't access this on a different thread from the one it was created on, so get back to GUI thread
            _synchronizationContext.Post(s => Peak = SelectedDevice.AudioMeterInformation
                .MasterPeakValue, null);
        }

        private void Stop()
        {
            _capture?.StopRecording();
            Peak = 0;
        }

        private void DisableRecordingPossibility(bool existsDevice)
        {
            if(existsDevice)
                return;

            RecordCommand.IsEnabled = false;
        }

        private void InitializeUsbDetector(UsbDeviceDetector deviceDetector)
        {
            _deviceDetector = deviceDetector;
            _deviceDetector.AddInsertionUSBHandler(UsbChangedState);
            _deviceDetector.AddRemovalUSBHandler(UsbChangedState);
            _deviceDetector.Start();
        }

        private void UsbChangedState(object sender, EventArrivedEventArgs eventArrivedEventArgs)
        {
            Application.Current.Dispatcher.Invoke(TryInitializeDevices);
        }

        private void GetDefaultRecordingFormat(MMDevice value)
        {
            if (value == null)
            {
                return;
            }
            using (var c = new WasapiCapture(value))
            {
                SampleTypeIndex = c.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat ? 0 : 1;
                SampleRate = c.WaveFormat.SampleRate;
                BitDepth = c.WaveFormat.BitsPerSample;
                ChannelCount = c.WaveFormat.Channels;
            }
        }

        public MMDevice SelectedDevice
        {
            get { return _wasapiCaptureModel.SelectedDevice; }
            set
            {
                _wasapiCaptureModel.SelectedDevice = value;
                GetDefaultRecordingFormat(value);
            }
        }

        public float Peak
        {
            get { return _wasapiCaptureModel.Peak; }
            set { _wasapiCaptureModel.Peak = value; }
        }

        public int SampleRate
        {
            get { return _wasapiCaptureModel.SampleRate; }
            set { _wasapiCaptureModel.SampleRate = value; }
        }

        public int BitDepth
        {
            get { return _wasapiCaptureModel.BitDepth; }
            set { _wasapiCaptureModel.BitDepth = value; }
        }

        public int ChannelCount
        {
            get { return _wasapiCaptureModel.ChannelCount; }
            set { _wasapiCaptureModel.ChannelCount = value; }
        }

        public bool IsBitDepthConfigurable => _wasapiCaptureModel.IsBitDepthConfigurable;

        public int SampleTypeIndex
        {
            get { return _wasapiCaptureModel.SampleTypeIndex; }
            set { _wasapiCaptureModel.SampleTypeIndex = value; }
        }

        public string Message
        {
            get { return _wasapiCaptureModel.Message; }
            set { _wasapiCaptureModel.Message = value; }
        }

        public int ShareModeIndex
        {
            get { return _wasapiCaptureModel.ShareModeIndex; }
            set { _wasapiCaptureModel.ShareModeIndex = value; }
        }

        public float RecordLevel
        {
            get { return _wasapiCaptureModel.RecordLevel; }
            set
            {
                _wasapiCaptureModel.RecordLevel = value;
                if (_capture != null)
                {
                    SelectedDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
                }
            }
        }

        public bool IsAnyDevice {
            get { return _wasapiCaptureModel.IsAnyDevice; }
            set
            {
                _wasapiCaptureModel.IsAnyDevice = value;
            }
        }

        public IEnumerable<MMDevice> CaptureDevices
        {
            get { return _wasapiCaptureModel.CaptureDevices; }
            private set
            {
                _wasapiCaptureModel.CaptureDevices = value;
            }
        }
    }
}
