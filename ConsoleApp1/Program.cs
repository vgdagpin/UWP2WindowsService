using System;
using System.Linq;
using System.IO.Pipes;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.AppService;
using Windows.Storage;

namespace ConsoleApp1
{
    class Program
    {
        static AppServiceConnection connection = null;


        static void Main(string[] args)
        {

            Thread appServiceThread = new Thread(new ThreadStart(ThreadProc));
            appServiceThread.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*****************************");
            Console.WriteLine("**** Classic desktop app ****");
            Console.WriteLine("*****************************");
            Console.ReadLine();
        }

        /// <summary>
        /// Creates the app service connection
        /// </summary>
        static async void ThreadProc()
        {
            var famName = ApplicationData.Current.LocalSettings.Values["param1"];

            connection = new AppServiceConnection();
            connection.AppServiceName = "ComTestService";
            connection.PackageFamilyName = famName.ToString();
            connection.RequestReceived += Connection_RequestReceived;

            AppServiceConnectionStatus status = await connection.OpenAsync();
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
