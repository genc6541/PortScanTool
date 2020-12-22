using NetTools;
using PortScanTool.Model;
using PortScanTool.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PortScanTool.Services
{
    public class IpHelperService : IIpHelperService
    {
        private const int MAX_PORT_NUMBER = 65535;
        public bool IsValidIPAddress(string host)
        {
            IPAddress ip;
            return IPAddress.TryParse(host, out ip);
        }

        public List<string> GetIpListFromRange(string startIpAddress, string endIpAddress) {

            List<string> IpList = new List<string>();
            var start = IPAddress.Parse(startIpAddress);
            var end = IPAddress.Parse(endIpAddress);

            var range = new IPAddressRange(start, end);

            foreach (var ip in range)
            {
                IpList.Add(ip.ToString());
            }

            return IpList;
        }

        public List<Address> GetIpListWithPorts(string startIpAddress, string endIpAddress) {

            List<Address> IpListWithPorts = new List<Address>();

            var ipList = GetIpListFromRange(startIpAddress, endIpAddress);

            foreach (var ipNumber in ipList) {

                for (int i = 1; i <= MAX_PORT_NUMBER; i++) {

                    Address address = new Address();
                    address.IpNumber = ipNumber;
                    address.Port = i;
                    address.Id = Guid.NewGuid().ToString();
                    IpListWithPorts.Add(address);                
                }
            
            }

            return IpListWithPorts;
        }

} 

}
