using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.Models
{
    public class DeviceIntegration
    {
        public DeviceIntegration(int deviceId, string deviceName, string deviceType, TimeSpan startTime, TimeSpan endTime, Boolean isRunning)
        {
            this.deviceId = deviceId;
            this.deviceName = deviceName; 
            this.deviceType = deviceType;
            this.startTime = startTime; 
            this.endTime = endTime;
            this.imageUlr = "toaster.png";

            this.workingDays = new bool[10];
            for(int i = 0; i < 7; i++)
            {
                workingDays[i] = i % 2 == 0;
               
            }
            turnOnLowest = true;
            this.isRunning = false;
        }
        
        private int deviceId { get; set; }

        public bool isRunning { get; set; }

        public string imageUlr { get; set; }

        public string deviceName { get; set; }

        public string deviceType { get; set; }
        public bool turnOnLowest { get; set; }
        
        public double runFor { get; set; }
        public TimeSpan startTime { get; set; }

        public TimeSpan endTime { get; set; }

        bool [] workingDays { get; set; }

        public DeviceIntegration Clone()
        {
            DeviceIntegration newObj = new DeviceIntegration(deviceId, deviceName, deviceType, startTime, endTime, isRunning);
            for(int i = 0; i < 7; i++){
                newObj.workingDays[i] = workingDays[i];
            }
            return newObj;

        }
    }
}
