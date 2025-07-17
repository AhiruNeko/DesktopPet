namespace Frontend {

    public class Message {
        public string Type { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public string Path { get; set; } = string.Empty;
        public string SoundPath { get; set; } = string.Empty;
        public bool PlaySound { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

}
    