using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NamedPipeClientStream pipeClient;
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public string FamilyName
        {
            get
            {
                return Package.Current.Id.FamilyName;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            // Messages.Add("Hello");
        }

        private async void StartService()
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                ApplicationData.Current.LocalSettings.Values["param1"] = FamilyName;

                Messages.Add("Starting service..");

                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();




                Messages.Add("Service started");
            }
        }

        private async void SendMessage()
        {
            try
            {
                Messages.Add("Connecting..");

                ValueSet valueSet = new ValueSet();
                valueSet.Add("request", MessageToSend.Text);

                if (App.Connection != null)
                {
                    AppServiceResponse response = await App.Connection.SendMessageAsync(valueSet);
                    MessageRecevied.Text = "Received response: " + response.Message["response"];
                }


                Messages.Add("Connected");
            }
            catch (Exception ex)
            {
                Messages.Add("Error");

                Messages.Add(ex.Message);
            }

        }
    }
}
