using PortScanTool.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace PortScanTool.Services
{
    public class PortScanService : IPostScanService
    {

        private readonly IIpHelperService ipHelperService;
        public static ObservableCollection<Address> ProccessedIpList;
        private static readonly object lockObj = new object();
        CancellationTokenSource tokenSource;
        CancellationToken cancellationToken;
        List<Task> tasks;
        List<Address> ipListWithPorts;


        public PortScanService(IIpHelperService ipHelperService)
        {
            this.ipHelperService = ipHelperService;
        }

        public async Task<ObservableCollection<Address>> GetProcessedIpList()
        {
            BindingOperations.EnableCollectionSynchronization(ProccessedIpList, lockObj);
            return ProccessedIpList;

        }

        public async Task<string> StartScanService(string startIpAddress, string endIpAddress, int taskCount, bool isRescanActive)
        {
            tasks = new List<Task>();

            // Was cancellation already requested?
          


            if (isRescanActive)
            {
                var processedItems = ProccessedIpList.Select(x => x.Id).ToList();
                ipListWithPorts = ipListWithPorts.Where(x => !processedItems.Contains(x.Id)).ToList();

                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
            }
            else {
                ipListWithPorts = ipHelperService.GetIpListWithPorts(startIpAddress, endIpAddress);

                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
            }


            var takeCount = ipListWithPorts.Count / taskCount;

            List<IEnumerable<Address>> listOfPartition = new List<IEnumerable<Address>>();
            for (int i = 0; i < ipListWithPorts.Count(); i += takeCount)
            {
                listOfPartition.Add(ipListWithPorts.Skip(i).Take(takeCount));
            }

            ProccessedIpList = new ObservableCollection<Address>();

            foreach (var item in listOfPartition)
            {

                tasks.Add(Task.Run(async () =>
               {

                   await ScanProcess(item);

               }, cancellationToken));

            }
            await Task.WhenAll(tasks);


            return Constants.Constants.ScanningCompletedMessage;
        }

        public async Task ScanProcess(IEnumerable<Address> ipPortList)
        {

            using (var tcpClient = new TcpClient())
            {
                foreach (var item in ipPortList)
                {

                    if (cancellationToken.IsCancellationRequested)
                    {
                        await Task.WhenAll(tasks.ToArray());
                        cancellationToken.ThrowIfCancellationRequested();

                    }

                    if (!item.CheckStatus)
                    {
                        try
                        {
                            await tcpClient.ConnectAsync(item.IpNumber, item.Port);
                            lock (lockObj)
                            {

                                item.PortStatus = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (lockObj)
                            {
                                item.PortStatus = false;
                            }
                        }
                        finally
                        {
                            lock (lockObj)
                            {
                                item.CheckStatus = true;

                                Address address = new Address
                                {
                                    Id = item.Id,
                                    IpNumber = item.IpNumber,
                                    Port = item.Port,
                                    CheckStatus = item.CheckStatus,
                                    PortStatus = item.PortStatus,
                                };


                                if (ProccessedIpList == null)
                                {
                                    ProccessedIpList = new ObservableCollection<Address>();
                                }
                                bool isContainsItem = ProccessedIpList.Any(x => x.Id == item.Id);

                                if (!isContainsItem)
                                {

                                    ProccessedIpList.Add(address);
                                }

                            }
                        }

                    }

                }

            }
        }

        public async Task<string> StopScanService()
        {
            tokenSource.Cancel();

            tokenSource.Dispose();
            return Constants.Constants.ProcessStoppedMessage;
        }


    }
}
