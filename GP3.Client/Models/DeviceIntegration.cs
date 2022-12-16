using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.Models
{
    public class DeviceIntegration : ObservableObject
    {
        public DeviceIntegration()
        {

        }
        public DeviceIntegration(int deviceId, string deviceName, string deviceType, TimeSpan startTime, TimeSpan endTime, bool isRunning, double runFor, bool turnOnLowest, string icon)
        {
            this.deviceId = deviceId;
            this.deviceType = deviceType;
            this.startTime = startTime;
            this.endTime = endTime;
            this.imageUlr = icon;
            this.turnOnLowest = turnOnLowest;
            this.runFor = runFor;

            this.DeviceName = deviceName;
            this.IsRunning = false;
        }
        public int deviceId { get; }
        public string imageUlr { get; set; }

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
            DeviceIntegration newObj = new DeviceIntegration(deviceId, deviceName, deviceType, startTime, endTime, isRunning, runFor, turnOnLowest, imageUlr);
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
            imageUlr = ToClone.imageUlr;
        }
    }
}