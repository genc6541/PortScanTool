using NetTools;
using PortScanTool.Commands;
using PortScanTool.Model;
using PortScanTool.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using Serilog;
using PortScanTool.Constants;

namespace PortScanTool.ViewModels
{
    public class PortScanViewModel : ViewModelBase
    {

        private readonly IPostScanService scanService;
        private readonly IIpHelperService ipHelperService;
        private bool IsStatusScanning = false;
        private readonly object _taskCountLock = new object();

        public PortScanViewModel(IPostScanService scanService, IIpHelperService ipHelperService)
        {
            this.scanService = scanService;
            this.ipHelperService = ipHelperService;

        }

        #region Commands
        private DelegateCommand startScanButtonCommand;
        private DelegateCommand stopScanButtonCommand;
        

        public ICommand StartScanButtonCommand
        {
            get
            {
                if (startScanButtonCommand == null)
                {
                    startScanButtonCommand = new DelegateCommand(
                       param => StartScanButtonPress(), param => CanStartScanButtonPress());
                }
                return startScanButtonCommand;
            }
        }
        public ICommand StopScanButtonCommand
        {
            get
            {
                if (stopScanButtonCommand == null)
                {
                    stopScanButtonCommand = new DelegateCommand(
                       param => StopScanButtonPress(), param => CanStopScanButtonPress());
                }
                return stopScanButtonCommand;
            }
        }

       
        #endregion

        #region Properties

        private string firstIpValue;
        public string FirstIpValue
        {
            get { return firstIpValue; }
            set
            {
                if (value != firstIpValue)
                {
                    firstIpValue = value;
                    OnPropertyChanged("FirstIpValue");
                }
            }
        }


        private string secondIpValue;
        public string SecondIpValue
        {
            get { return secondIpValue; }
            set
            {
                if (value != secondIpValue)
                {
                    secondIpValue = value;
                    OnPropertyChanged("SecondIpValue");
                }
            }
        }

        private int taskCount = 1;
        public int TaskCount
        {
            get { return taskCount; }
            set
            {
                if (value != taskCount)
                {
                    taskCount = value;
                    OnPropertyChanged("TaskCount");
                    lock (_taskCountLock)
                        Monitor.PulseAll(_taskCountLock);
                    Task.Run(() =>
                    {
                        lock (_taskCountLock)
                            if (!Monitor.Wait(_taskCountLock, 2000))
                            {
                                if (IsStatusScanning)
                                {
                                    StartReScan();
                                }
                            }
                    });
                   
                }
            }
        }

        private string scanStatusInformation;
        public string ScanStatusInformation
        {
            get { return scanStatusInformation; }
            set
            {
                if (value != scanStatusInformation)
                {
                    scanStatusInformation = value;
                    OnPropertyChanged("ScanStatusInformation");
                }
            }
        }


        private bool startButtonEnabled = true;
        public bool StartButtonEnabled {
            get { return startButtonEnabled; }
            set
            {
                if (value != startButtonEnabled)
                {
                    startButtonEnabled = value;
                    OnPropertyChanged("StartButtonEnabled");
                }
            }
        }

        private bool stopButtonEnabled = true;
        public bool StopButtonEnabled
        {
            get { return stopButtonEnabled; }
            set
            {
                if (value != stopButtonEnabled)
                {
                    stopButtonEnabled = value;
                    OnPropertyChanged("StopButtonEnabled");
                }
            }
        }

     


        private ObservableCollection<Address> processedIpPortList;
        public ObservableCollection<Address> ProcessedIpPortList
        {
            get { return processedIpPortList; }
            set
            {
                if (value != processedIpPortList)
                {
                    processedIpPortList = value;
                    OnPropertyChanged("ProcessedIpPortList");
                
                }
            }
        }


        #endregion

        #region Methods


        private bool CanStopScanButtonPress()
        {
            return true;
        }

        private static bool CanStartScanButtonPress()
        {
            return true;
        }

        public async Task StartScanButtonPress()
        {
            try
            {
               
                ValidateIpAddresValues();
                if (ProcessedIpPortList != null) {
                    ObservableCollection<Address> clearList = new ObservableCollection<Address>();
                    ProcessedIpPortList = clearList;
                }

                ScanStatusInformation = Constants.Constants.ProcessStartedMessage;
               
                ShowMessageAndLog(Constants.Constants.ScanningStartedMessage);
                StartButtonEnabled = false;
                IsStatusScanning = true;
                var result =  await scanService.StartScanService(this.firstIpValue, this.SecondIpValue, this.TaskCount, false);        
                ScanStatusInformation = Constants.Constants.ProcessCompletedMessage;
                Log.Information(ScanStatusInformation);

                StartButtonEnabled = true;
                ShowMessageAndLog(result);
                ProcessedIpPortList = scanService.GetProcessedIpList().Result;
                
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public async Task StopScanButtonPress()
        {
            try
            {

                IsStatusScanning = false;
                string result = await scanService.StopScanService();
                ScanStatusInformation = result;
                var ipListResult =  scanService.GetProcessedIpList().Result;
                ProcessedIpPortList = ipListResult;
                ShowMessageAndLog(result);
                StartButtonEnabled = true;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public async Task StartReScan()
        {

            ValidateIpAddresValues();
            ShowMessageAndLog(Constants.Constants.ProcessReStartedMessage);
            var result = await scanService.StartScanService(this.firstIpValue, this.SecondIpValue, this.TaskCount, true);
            ShowMessageAndLog(result);
            ProcessedIpPortList = scanService.GetProcessedIpList().Result;
            
        }

        public void ValidateIpAddresValues() {
            if (!ipHelperService.IsValidIPAddress(this.FirstIpValue) || !ipHelperService.IsValidIPAddress(this.SecondIpValue))
            {
                MessageBox.Show(Constants.Constants.InvalidIpErrorMessage);
                return;
            }

        }

        public void ShowMessageAndLog(string message) {

            MessageBox.Show(message);
            Log.Information(message);
        }
        #endregion

    }
}
