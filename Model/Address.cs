using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PortScanTool.Model
{
    public class Address : INotifyPropertyChanged
    {

        public string id { get; set; }
        public string ipNumber;
        public int port;
        public bool portStatus;
        public bool checkStatus;


        public string Id
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("Id"); }
        }

        public string IpNumber
        {
            get { return ipNumber; }
            set { ipNumber = value; NotifyPropertyChanged("IpNumber"); }
        }
        public int Port
        {
            get { return port; }
            set { port = value; NotifyPropertyChanged("Port"); }
        }


        public bool PortStatus
        {
            get { return portStatus; }
            set { portStatus = value; NotifyPropertyChanged("PortStatus"); }
        }

        public bool CheckStatus
        {
            get { return checkStatus; }
            set { checkStatus = value; NotifyPropertyChanged("CheckStatus"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
