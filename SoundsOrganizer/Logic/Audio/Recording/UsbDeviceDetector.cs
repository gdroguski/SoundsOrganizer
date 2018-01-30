using System;
using System.Management;
using System.Windows;

namespace SoundsOrganizer.Logic.Audio.Recording
{
    public class UsbDeviceDetector
    {
        private ManagementEventWatcher _insertionEventWatcher;
        private ManagementEventWatcher _removalEventWatcher;

        public UsbDeviceDetector()
        {
            AppDomain.CurrentDomain.ProcessExit += CleanUp;
        }

        private void CleanUp(object sender, EventArgs e)
        {
            if (_removalEventWatcher == null || _insertionEventWatcher == null)
                return;
            
            _removalEventWatcher.Dispose();
            _insertionEventWatcher.Dispose();
        }

        public void Start()
        {
            if (_removalEventWatcher == null || _insertionEventWatcher == null)
                return;
            
            _removalEventWatcher.Start();
            _insertionEventWatcher.Start();
        }

        public void AddInsertionUSBHandler(EventArrivedEventHandler eventArrived)
        {
            _insertionEventWatcher = InitializeHandler(eventArrived, _insertionEventWatcher, "__InstanceCreationEvent");
        }

        public void AddRemovalUSBHandler(EventArrivedEventHandler eventArrived)
        {
            _removalEventWatcher = InitializeHandler(eventArrived, _removalEventWatcher, "__InstanceDeletionEvent");
        }

        private ManagementEventWatcher InitializeHandler(EventArrivedEventHandler eventArrived, ManagementEventWatcher eventWatcher,
            string eventClassName)
        {
            var managementScope = new ManagementScope("root\\CIMV2") { Options = { EnablePrivileges = true } };

            try
            {
                var wqlEventQuery = new WqlEventQuery
                {
                    EventClassName = eventClassName,
                    WithinInterval = new TimeSpan(0, 0, 3),
                    Condition = "TargetInstance ISA 'Win32_USBControllerdevice'"
                };
                eventWatcher = new ManagementEventWatcher(managementScope, wqlEventQuery);
                eventWatcher.EventArrived += eventArrived;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                eventWatcher.Stop();
            }

            return eventWatcher;
        }
    }
}
