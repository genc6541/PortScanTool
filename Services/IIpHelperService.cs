using PortScanTool.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PortScanTool.Services
{
    public interface IIpHelperService
    {
        bool IsValidIPAddress(string host);

        List<string> GetIpListFromRange(string startIpAddress, string endIpAddress);

        List<Address> GetIpListWithPorts(string startIpAddress, string endIpAddress);

    }
}
