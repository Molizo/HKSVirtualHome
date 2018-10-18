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
using MQTTnet;
using MQTTnet.Client;

namespace HKSVirtualHome
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                                .WithClientId("HKS Virtual Home Software")
                                .WithTcpServer("localhost", 8883)
                                .WithTls()
                                .WithCleanSession()
                                .Build();

            mqttClient.Disconnected += async (s, e) =>
            {
                Console.WriteLine("[System] Disconnected from server");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("[System] Connection failed");
                    Console.WriteLine("[System] Please check your connection and credentials");
                }
            };
            mqttClient.Connected += async (s, e) =>
            {
                Console.WriteLine("[System] Connected to server");
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("/HKS/").Build());
                Console.WriteLine("[System] Waiting for messages");
            };
            mqttClient.ApplicationMessageReceived += (s, e) =>
            {
                Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            };
            MqttConnect(mqttClient, options).Wait();
        }
        private static async Task MqttConnect(IMqttClient mqttClient, IMqttClientOptions options)
        {
            Console.WriteLine("[System] Attempting to connect to server...");
            await mqttClient.ConnectAsync(options);
        }
    }
}
