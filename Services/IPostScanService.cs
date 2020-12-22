using PortScanTool.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace PortScanTool.Services
{
    public interface IPostScanService
    {
        Task<string> StartScanService(string startIpAddress, string endIpAddress, int taskCount, bool isRecanActive);
        Task ScanProcess(IEnumerable<Address> concurrentQueue );

        Task<string> StopScanService();

        Task<ObservableCollection<Address>> GetProcessedIpList();
    }
}
