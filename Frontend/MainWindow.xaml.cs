using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientWebSocket _socket = new ClientWebSocket();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private byte[] _recvBuffer = new byte[1024];

        private string path;
        private double scale;
        private double x;
        private double y;
        private double mouseX;
        private double mouseY;
        private bool mouseLeftDown;
        private bool mouseLeftUp;
        private bool mouseRightDown;
        private bool mouseRightUp;
        private bool mouseMove;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.path = @"../../../../UserPets/DemoPet/1.png";
            this.DisplayImage();
            this.Width = 100;
            this.Height = 100;
            this.scale = 1.0;
            this.x = this.Left;
            this.y = this.Top;
            this.GetMousePos();
            this.mouseLeftDown = false;
            this.mouseLeftUp = false;
            this.mouseRightDown = false;
            this.mouseRightUp = false;
            this.mouseMove = false;

            await ConnectAndRunAsync();
        }

        private async Task ConnectAndRunAsync()
        {
            try
            {
                await _socket.ConnectAsync(new Uri("ws://localhost:8765"), _cts.Token);

                var initData = new
                {
                    type = "init",
                    path = this.path,
                    width = this.Width,
                    height = this.Height,
                    scale = this.scale,
                    x = this.x,
                    y = this.y,
                    mouseX = this.mouseX,
                    mouseY = this.mouseY,
                    mouseLeftDown = this.mouseLeftDown,
                    mouseLeftUp = this.mouseLeftUp,
                    mouseRightDown = this.mouseRightDown,
                    mouseRightUp = this.mouseRightUp,
                    mouseMove = this.mouseMove
                };

                string initJson = JsonSerializer.Serialize(initData);
                byte[] initBuffer = Encoding.UTF8.GetBytes(initJson);
                await _socket.SendAsync(new ArraySegment<byte>(initBuffer), WebSocketMessageType.Text, true, _cts.Token);

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

        private void DisplayImage()
        {
            string imagePath = Path.GetFullPath(this.path);
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

        private void GetMousePos()
        { 
            Point mPosition = PointToScreen(Mouse.GetPosition(this));
            this.mouseX = mPosition.X;
            this.mouseY = mPosition.Y;
        }

    }
}