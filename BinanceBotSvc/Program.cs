using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using BinanceBotLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BinanceBotSvc
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseWindowsService().ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            });

        internal static string UserName { get; set; }
        internal static string Password { get; set; }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                CreateHostBuilder(args).Build().Run();
            }
            else
            {
                switch (args[0])
                {
                    case "-install":
                        UserName = args[1];
                        Password = args[2];
                        InstallService();
                        StartService();
                        break;
                    case "-uninstall":
                        StopService();
                        UninstallService();
                        break;
                    case "-start":
                        StartService();
                        break;
                    case "-stop":
                        StopService();
                        break;
                    case "-restart":
                        RestartService();
                        break;
                }
            }
        }

        private static bool IsInstalled()
        {
            using (ServiceController controller = new ServiceController("BinanceBotSvc"))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private static void InstallService()
        {
            if (IsInstalled()) return;

            try
            {
            }
            catch
            {
                throw;
            }
        }

        private static void UninstallService()
        {
            if (!IsInstalled()) return;
            try
            {
            }
            catch
            {
                throw;
            }
        }

        public static void StartService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller = new ServiceController("BinanceBotSvc"))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void StopService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller = new ServiceController("BinanceBotSvc"))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void RestartService()
        {
            if (!IsInstalled()) return;

            StopService();
            StartService();
        }
    }
}