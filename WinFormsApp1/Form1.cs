using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        AppServiceConnection connection = null;
        string PackageFamilyName = null;
        string AppServiceName = "ComTestService";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PackageFamilyName = ApplicationData.Current.LocalSettings.Values["param1"]?.ToString();

            connection = new AppServiceConnection
            {
                AppServiceName = AppServiceName,
                PackageFamilyName = PackageFamilyName
            };

            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            new Thread(new ThreadStart(StartConnect)).Start();
        }



        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            LogToListView("Service closed");
        }

        private async void StartConnect()
        {
            AppServiceConnectionStatus status = await connection.OpenAsync();

            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    LogToListView("Connection established - waiting for requests");
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    LogToListView("The app AppServicesProvider is not installed.");
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    LogToListView("The app AppServicesProvider is not available.");
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    LogToListView(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                    return;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    LogToListView(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return;
            }

            LogToListView($"Connecting to {PackageFamilyName} : {AppServiceName}");

            Console.ReadLine();
        }

        private void LogToListView(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogToListView), new object[] { message });
                return;
            }

            listBox1.Items.Add($"{DateTime.Now:MM/dd/yyyy hh:mm:ss tt} - {message}");

            notifyIcon.BalloonTipText = $"{DateTime.Now:MM/dd/yyyy hh:mm:ss tt} - {message}";
            notifyIcon.ShowBalloonTip(1000);
        }

        /// <summary>
        /// Receives message from UWP app and sends a response back
        /// </summary>
        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            string key = args.Request.Message.First().Key;
            string value = args.Request.Message.First().Value.ToString();
            Console.ForegroundColor = ConsoleColor.Cyan;
            LogToListView(string.Format("Received message '{0}' with value '{1}'", key, value));
            if (key == "request")
            {
                ValueSet valueSet = new ValueSet();
                valueSet.Add("response", $"{DateTime.Now:MM/dd/yyyy hh:mm:ss tt} - {value}");
                Console.ForegroundColor = ConsoleColor.White;
                LogToListView(string.Format("Sending response: '{0}'", value.ToUpper()));
                args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //to minimize window
            this.WindowState = FormWindowState.Minimized;

            //to hide from taskbar
            this.Hide();

            notifyIcon.Visible = true;
        }
    }
}
