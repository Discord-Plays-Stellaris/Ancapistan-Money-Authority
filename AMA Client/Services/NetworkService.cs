using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AMA_Client.Services
{
    static class NetworkService
    {
        public static string MAC { get { return GetMAC(); } }

        private static string GetMAC()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
        }
    }
}
