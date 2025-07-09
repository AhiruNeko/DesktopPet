using System.IO;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
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

        private PetStatus status = new PetStatus();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.status.Path = @"../../../../UserPets/DemoPet/1.png";
            this.DisplayImage();
            this.status.Width = PetImage.Source.Width;
            this.status.Height = PetImage.Source.Height;    
            this.status.X = this.Left;
            this.status.Y = this.Top;
            this.UpdateMousePos();

            await ConnectAndRunAsync();
        }

        private async Task ConnectAndRunAsync()
        {
            try
            {
                await _socket.ConnectAsync(new Uri("ws://localhost:8765"), _cts.Token);

                PetStatus initData = this.status;

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
                this.UpdateData();

                string json = JsonSerializer.Serialize(this.status);
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

        private void UpdateData()
        {   
            this.status.Type = "update";
            this.UpdateMousePos();
        }

        private void ExcuteCommand(object command)
        { 
            
        }

        private void DisplayImage()
        {
            string imagePath = Path.GetFullPath(this.status.Path);
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private void UpdateMousePos()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                this.status.MouseX = p.X;
                this.status.MouseY = p.Y;
            }
            else
            {
                this.status.MouseX = -1;
                this.status.MouseY = -1;
            }
        }

    }
}