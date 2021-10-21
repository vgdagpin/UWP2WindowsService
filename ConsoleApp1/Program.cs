using System;
using System.Linq;
using System.IO.Pipes;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.AppService;
using Windows.Storage;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static AppServiceConnection connection = null;
        static string PackageFamilyName = null;
        static string AppServiceName = "ComTestService";


        static async Task Main(string[] args)
        {
            PackageFamilyName = ApplicationData.Current.LocalSettings.Values["param1"]?.ToString();

            connection = new AppServiceConnection
            {
                AppServiceName = AppServiceName,
                PackageFamilyName = PackageFamilyName
            };

            connection.RequestReceived += Connection_RequestReceived;

            AppServiceConnectionStatus status = await connection.OpenAsync();

            Console.WriteLine();

            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection established - waiting for requests");
                    Console.WriteLine();
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not installed.");
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not available.");
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                    return;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return;
            }

            Console.WriteLine($"Connecting to {PackageFamilyName} : {AppServiceName}");

            Console.ReadLine();
        }

        /// <summary>
        /// Creates the app service connection
        /// </summary>
        static void ThreadProc()
        {
            connection = new AppServiceConnection
            {
                AppServiceName = AppServiceName,
                PackageFamilyName = PackageFamilyName
            };

            connection.RequestReceived += Connection_RequestReceived;

            AppServiceConnectionStatus status = connection.OpenAsync().GetResults();
            Console.WriteLine();

            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection established - waiting for requests");
                    Console.WriteLine();
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not installed.");
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not available.");
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                    return;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return;
            }
        }

        /// <summary>
        /// Receives message from UWP app and sends a response back
        /// </summary>
        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            string key = args.Request.Message.First().Key;
            string value = args.Request.Message.First().Value.ToString();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(string.Format("Received message '{0}' with value '{1}'", key, value));
            if (key == "request")
            {
                ValueSet valueSet = new ValueSet();
                valueSet.Add("response", value.ToUpper());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(string.Format("Sending response: '{0}'", value.ToUpper()));
                Console.WriteLine();
                args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            }
        }
    }
}
