using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path;
        private ClientWebSocket _socket = new ClientWebSocket();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private byte[] _recvBuffer = new byte[1024];


        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (_, __) =>
            {
                await ConnectAndRunAsync();
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = PetImage.Source.Width;
            this.Height = PetImage.Source.Height;
        }

        private async Task ConnectAndRunAsync()
        {
            try
            {
                await _socket.ConnectAsync(new Uri("ws://localhost:8765"), _cts.Token);

                _ = Task.Run(SendLoop);
                _ = Task.Run(ReceiveLoop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Failed: " + ex.Message);
            }
        }

        private async Task SendLoop()
        {
            while (_socket.State == WebSocketState.Open)
            {
                var data = GetData();

                string json = JsonSerializer.Serialize(data);
                byte[] buffer = Encoding.UTF8.GetBytes(json);

                await _socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cts.Token);
                await Task.Delay(50);
            }
        }

        private async Task ReceiveLoop()
        {
            while (_socket.State == WebSocketState.Open)
            {
                var result = await _socket.ReceiveAsync(new ArraySegment<byte>(_recvBuffer), _cts.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(_recvBuffer, 0, result.Count);
                    Dispatcher.Invoke(() =>
                    {
                        ExcuteCommand(message);
                    });
                }
            }
        }

        private object GetData()
        {
            return new { type = "update" };
        }

        private void ExcuteCommand(object command)
        { 
            
        }

        private void DisplayImage(string path)
        {
            string imagePath = Path.GetFullPath(path);
            if (File.Exists(imagePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                PetImage.Source = bitmap;
            }
            else
            {
                MessageBox.Show("Invalid Path");
            }
        }

        private void SetPos(int x, int y)
        { 
            this.Left = x;
            this.Top = y;
        }

        private double[] GetMousePos()
        { 
            Point mPosition = PointToScreen(Mouse.GetPosition(this));
            return new double[] { mPosition.X, mPosition.Y };
        }

    }
}