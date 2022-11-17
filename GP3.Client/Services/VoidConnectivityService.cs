using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.Services
{
    public class VoidConnectivityService : IConnectivityService
    {
        public bool IsConnected()
        {
            return true;
        }
    }
}
