using PortScanTool.Services;
using PortScanTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PortScanTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PortScan : Window
    {
        private readonly IPostScanService scanService;
        private readonly IIpHelperService ipHelperService;
        public PortScan(IPostScanService scanService, IIpHelperService ipHelperService)
        {
            this.scanService = scanService;
            this.ipHelperService = ipHelperService;


            InitializeComponent();

            DataContext = new PortScanViewModel(scanService, ipHelperService);

        }

        private void TaskCountText_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void RunningTaskSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
