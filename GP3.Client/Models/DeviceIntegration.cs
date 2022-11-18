using CommunityToolkit.Mvvm.ComponentModel;

namespace GP3.Client.Models
{
    public class DeviceIntegration : ObservableObject
    {
        public DeviceIntegration()
        {

        }
        public DeviceIntegration(int deviceId, string deviceName, string deviceType, TimeSpan startTime, TimeSpan endTime, bool isRunning, double runFor, bool turnOnLowest)
        {
            this.deviceId = deviceId;
            this.deviceType = deviceType;
            this.startTime = startTime; 
            this.endTime = endTime;
            imageUlr = "toaster.png";
            this.turnOnLowest = turnOnLowest;
            this.runFor = runFor;

            DeviceName = deviceName; 
            IsRunning = false;
        }
        public int deviceId { get; }
        public string imageUlr { get; }

        public string deviceType { get; set; }
        public bool turnOnLowest { get; set; }
        public double runFor { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }

        private string deviceName;
        private bool isRunning;
        public string DeviceName
        {
            get => deviceName;
            set => SetProperty(ref deviceName, value);
        }
        public bool IsRunning
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value);
        }

        public DeviceIntegration Clone()
        {
            DeviceIntegration newObj = new DeviceIntegration(deviceId, deviceName, deviceType, startTime, endTime, isRunning, runFor, turnOnLowest);
            return newObj;
        }

        public void Clone(DeviceIntegration ToClone)
        {
            DeviceName = ToClone.DeviceName;
            deviceType = ToClone.deviceType;
            turnOnLowest = ToClone.turnOnLowest;
            runFor = ToClone.runFor;
            startTime = ToClone.startTime;
            endTime = ToClone.endTime;
        }
    }
}
