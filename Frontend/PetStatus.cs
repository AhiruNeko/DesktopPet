namespace Frontend
{
    public class PetStatus
    {   
        public string Type { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Path { get; set; }
        public double Scale { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double MouseX { get; set; }
        public double MouseY { get; set; }
        public bool MouseLeftDown { get; set; }
        public bool MouseLeftUp { get; set; }
        public bool MouseRightDown { get; set; }
        public bool MouseRightUp { get; set; }
        public bool MouseMove { get; set; }
        public PetStatus()
        {
            Type = "init";
            Path = string.Empty;
            Width = 0.0;
            Height = 0.0;
            Scale = 1.0;
            X = 0.0;
            Y = 0.0;
            MouseX = 0.0;
            MouseY = 0.0;
            MouseLeftDown = false;
            MouseLeftUp = false;
            MouseRightDown = false;
            MouseRightUp = false;
            MouseMove = false;
        }
    }
}
