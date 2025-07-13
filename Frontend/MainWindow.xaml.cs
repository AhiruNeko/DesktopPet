using System.Drawing;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Gma.System.MouseKeyHook;

namespace Frontend {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ClientWebSocket _socket = new ClientWebSocket();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private byte[] _recvBuffer = new byte[1024];

        public PetStatus status = new PetStatus();
        private NotifyIcon _trayIcon;
        private IKeyboardMouseEvents _globalHook;
        private DateTime _lastSendTime = DateTime.MinValue;

        public MainWindow() {
            InitializeComponent();
            InitializeTrayIcon();
            this.ShowInTaskbar = false;
        }

        private void InitializeTrayIcon() {
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = SystemIcons.Application;
            _trayIcon.Visible = true;
            _trayIcon.Text = "Desktop Pet";
            var menu = new ContextMenuStrip();

            ToolStripMenuItem _TopmostItem = new ToolStripMenuItem("Topmost", null, (s, e) => {
                this.Topmost = !this.Topmost;
            });
            _TopmostItem.Checked = this.Topmost;
            _TopmostItem.CheckOnClick = true;

            menu.Items.Add(_TopmostItem);
            menu.Items.Add("Exit", null, (s, e) => {
                _cts.Cancel();
                _ = CloseWebSocket();
                _trayIcon.Visible = false;
                System.Windows.Application.Current.Shutdown();
            });
            _trayIcon.ContextMenuStrip = menu;
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e) {
            this.status.Path = @"../../../../UserPets/DemoPet/assets/1.png";
            this.DisplayImage();
            this.status.Width = PetImage.Source.Width;
            this.status.Height = PetImage.Source.Height;    
            this.status.X = this.Left;
            this.status.Y = this.Top;
            this.UpdateMousePos();

            StartGlobalMouseHook();
            await ConnectAsync();
        }

        public async Task SendDataAsync() {
            if (_socket.State == WebSocketState.Open) {
                try {
                    string json = JsonSerializer.Serialize(this.status);
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    await _socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cts.Token);
                    Console.WriteLine("Sending: " + json);
                }
                catch (Exception ex) {
                    Console.WriteLine("Send Error: " + ex.Message);
                }
            }
            else {
                Console.WriteLine("Socket is not open.");
            }
        }

        private async Task ReceiveLoopAsync() {
            try {
                while (_socket.State == WebSocketState.Open) {
                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(_recvBuffer), _cts.Token);

                    if (result.MessageType == WebSocketMessageType.Text) {
                        string message = Encoding.UTF8.GetString(_recvBuffer, 0, result.Count);
                        System.Windows.Application.Current.Dispatcher.Invoke(() => {
                            HandleIncomingMessage(message);
                        });
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Receive Error: " + ex.Message);
            }
        }

        public async Task ConnectAsync() {
            try {
                await _socket.ConnectAsync(new Uri("ws://localhost:8765"), _cts.Token);

                _ = Task.Run(ReceiveLoopAsync);

                Console.WriteLine("WebSocket Connected.");
            }
            catch (Exception ex) {
                Console.WriteLine("Connection Error: " + ex.Message);
            }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            base.OnClosing(e);
            _ = CloseWebSocket();
        }

        private async Task CloseWebSocket() {
            if (_socket != null && _socket.State == WebSocketState.Open) {
                try {
                    await _socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None
                    );
                }
                catch (Exception ex) {
                    Console.WriteLine("Error during WebSocket close: " + ex.Message);
                }
            }
        }

        private void StartGlobalMouseHook() {
            _globalHook = Hook.GlobalEvents();
            _globalHook.MouseDown += PetMouseDown;
            _globalHook.MouseUp += PetMouseUp;
            _globalHook.MouseMove += PetMouseMove;
        }

        private async void PetMouseDown(object sender, MouseEventArgs e) {
            this.UpdateGeneralStatus();
            if (e.Button == MouseButtons.Left) {
                this.status.MouseLeftDown = true;
                this.status.MouseLeftPressed = true;
            }
            else if (e.Button == MouseButtons.Right) {
                this.status.MouseRightDown = true;
                this.status.MouseRightPressed = true;
            }
            await SendDataAsync();
            this.status.MouseLeftDown = false;
            this.status.MouseRightDown = false;
        }

        private async void PetMouseUp(object sender, MouseEventArgs e) {
            this.UpdateGeneralStatus();
            if (e.Button == MouseButtons.Left) {
                this.status.MouseLeftUp = true;
                this.status.MouseLeftPressed = false;
            }
            else if (e.Button == MouseButtons.Right) {
                this.status.MouseRightUp = true;
                this.status.MouseRightPressed = false;
            }
            await SendDataAsync();
            this.status.MouseLeftUp = false;
            this.status.MouseRightUp = false;
        }

        private async void PetMouseMove(object sender, MouseEventArgs e) {
            var now = DateTime.Now;
            if ((now - _lastSendTime).TotalMilliseconds < 30)
                return;

            _lastSendTime = now;

            this.UpdateGeneralStatus();
            this.status.MouseMove = true;
            await SendDataAsync();
            this.status.MouseMove = false;
        }

        private void ExcuteCommand(object command) { 
            
        }

        private void DisplayImage() {
            string imagePath = Path.GetFullPath(this.status.Path);
            if (File.Exists(imagePath)) {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                PetImage.Source = bitmap;
            }
            else {
                Console.WriteLine("Invalid Path");
            }
        }

        private void SetPos(int x, int y) { 
            this.Left = x;
            this.Top = y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int X;
            public int Y;
        }

        private void UpdateMousePos() {
            POINT p;
            if (GetCursorPos(out p)) {
                this.status.MouseX = p.X;
                this.status.MouseY = p.Y;
            }
            else {
                this.status.MouseX = -1;
                this.status.MouseY = -1;
            }
        }

        private void UpdateGeneralStatus() {
            this.status.Type = "update";
            this.status.Width = PetImage.Source.Width;
            this.status.Height = PetImage.Source.Height;
            this.status.X = this.Left;
            this.status.Y = this.Top;
            this.UpdateMousePos();
        }

        private void MouseMoveOverPet(object sender, System.Windows.Input.MouseEventArgs e) {
            this.status.MouseOverPet = true;
        }

        private void MouseLeavePet(object sender, System.Windows.Input.MouseEventArgs e) {
            this.status.MouseOverPet = false;
        }

        private void HandleIncomingMessage(String message) { 
        }

    }
}