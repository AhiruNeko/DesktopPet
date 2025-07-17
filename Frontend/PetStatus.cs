namespace Frontend {
    public class PetStatus {
        public string Type { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public string Path { get; set; } = string.Empty;
        public string SoundPath { get; set; } = string.Empty;
        public bool PlaySound { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double MouseX { get; set; }
        public double MouseY { get; set; }
        public bool MouseLeftDown { get; set; }
        public bool MouseLeftUp { get; set; }
        public bool MouseRightDown { get; set; }
        public bool MouseRightUp { get; set; }
        public bool MouseLeftPressed { get; set; }
        public bool MouseRightPressed { get; set; }
        public bool MouseMove { get; set; }
        public bool MouseOverPet { get; set; }
    }
}

