using Microsoft.Extensions.DependencyInjection;
using PortScanTool.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PortScanTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly IServiceProvider _serviceProvider;
        public App()
        {
            Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("log.txt")
    .CreateLogger();


            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);


            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IIpHelperService>(provider => new IpHelperService());
            services.AddSingleton<IPostScanService>(provider => new PortScanService(provider.GetRequiredService<IIpHelperService>()));
            services.AddSingleton<PortScan>();
            

        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<PortScan>();
            mainWindow.Show();
        }

    }
}
