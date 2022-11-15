using CommunityToolkit.Mvvm.ComponentModel;
using GP3.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.ViewModels
{
    [QueryProperty("Devices", "Devices")]
    [QueryProperty("Device", "Device")]
    public partial class EditDeviceViewModel: BaseViewModel
    {

        public EditDeviceViewModel()
        {
            Title = "Update Device";
        }


        [ObservableProperty]
        public DeviceIntegration device;

        [ObservableProperty]
        public ObservableCollection<DeviceIntegration> devices;

    }
}
