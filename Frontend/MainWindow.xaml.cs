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
using WpfAnimatedGif;

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
        
        private TaskCompletionSource<Message> _initSignal = new();
        private bool _initialized = false;

        private double _initX, _initY, _initWidth, _initHeight;
        private string _initPath;


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
            menu.Items.Add("Reset", null, async (s, e) => {
                this.SetLeft(_initX);
                this.SetTop(_initY);
                this.Width = _initWidth;
                this.Height = _initHeight;
                this.status.Path = this._initPath;
                this.Display();
                this.UpdateGeneralStatus("reset");
                await SendDataAsync();
            });
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e) {
            await ConnectAsync();

            Console.WriteLine("Waiting for init signal...");
            Message initData = await _initSignal.Task;
            
            this._initX = initData.X;
            this._initY = initData.Y;
            this._initWidth = initData.Width;
            this._initHeight = initData.Height;
            this._initPath = initData.Path;
            
            this.SetLeft(initData.X);
            this.SetTop(initData.Y);
            this.Width = initData.Width;
            this.Height = initData.Height;
            this.status.Path = initData.Path;
            this.Display();
            this.UpdateGeneralStatus();

            Console.WriteLine("Init completed.");
            StartGlobalMouseHook();
        }

        private async Task SendDataAsync() {
            if (!_initialized) {
                Console.WriteLine("Init not completed. Send aborted.");
                return;
            }

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

                        if (!_initialized)
                        {
                            try
                            {
                                var msg = ParseMessage(message);
                                if (msg.Type == "init")
                                {
                                    _initialized = true;
                                    _initSignal.TrySetResult(msg);
                                    Console.WriteLine("Init message received.");
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Init parse error: " + ex.Message);
                            }
                        }

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

        private void Display()
        {
            string imagePath = Path.GetFullPath(this.status.Path);
            if (File.Exists(imagePath))
            {
                var imageUri = new Uri(imagePath);
                var image = new BitmapImage(imageUri);
                ImageBehavior.SetAnimatedSource(PetImage, image);
            }
            else
            {
                Console.WriteLine("Invalid Path");
            }
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

        private void UpdateGeneralStatus(string updateType="update") {
            this.status.Type = updateType;
            this.status.Width = this.Width;
            this.status.Height = this.Height;
            this.status.X = this.GetLeft();
            this.status.Y = this.GetTop();
            this.UpdateMousePos();
        }

        private void MouseMoveOverPet(object sender, System.Windows.Input.MouseEventArgs e) {
            this.status.MouseOverPet = true;
        }

        private void MouseLeavePet(object sender, System.Windows.Input.MouseEventArgs e) {
            this.status.MouseOverPet = false;
        }
        
        private (double dpiX, double dpiY) GetDpiFactors() {
            PresentationSource source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget != null) {
                return (source.CompositionTarget.TransformToDevice.M11,
                    source.CompositionTarget.TransformToDevice.M22);
            }
            return (1.0, 1.0);
        }
        
        private void SetLeft(double physicalPixelX) {
            var (dpiX, _) = GetDpiFactors();
            this.Left = physicalPixelX / dpiX;
        }

        private void SetTop(double physicalPixelY) {
            var (_, dpiY) = GetDpiFactors();
            this.Top = physicalPixelY / dpiY;
        }

        private double GetLeft() {
            var (dpiX, _) = GetDpiFactors();
            return this.Left * dpiX;
        }
        
        private double GetTop() {
            var (_, dpiY) = GetDpiFactors();
            return this.Top * dpiY;
        }
        
        public static Message ParseMessage(string json) {
            try {
                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = false
                };
                var msg = JsonSerializer.Deserialize<Message>(json, options);
                if (msg == null) {
                    throw new JsonException("Failed to deserialize JSON into Message object.");
                }
                return msg;
            }
            catch (Exception ex) {
                throw new JsonException("Error during JSON deserialization: " + ex.Message, ex);
            }
        }

        private void HandleIncomingMessage(string message) { 
            Message msg = ParseMessage(message);
            if (msg.Type == "update") {
                if (this.status.Path != msg.Path) {
                    this.status.Path = msg.Path;
                    this.Display();
                }
                this.Height = msg.Height;
                this.Width = msg.Width;
                this.SetLeft(msg.X);
                this.SetTop(msg.Y);
                this.UpdateGeneralStatus();
            }
        }

    }
}