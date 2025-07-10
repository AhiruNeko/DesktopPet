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
        public bool MouseOverPet { get; set; }
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
            MouseOverPet = false;
        }

        public void copy(PetStatus AnotherStatus)
        { 
            this.Type = AnotherStatus.Type;
            this.Path = AnotherStatus.Path;
            this.Width = AnotherStatus.Width;
            this.Height = AnotherStatus.Height;
            this.Scale = AnotherStatus.Scale;
            this.X = AnotherStatus.X;
            this.Y = AnotherStatus.Y;
            this.MouseX = AnotherStatus.MouseX;
            this.MouseY = AnotherStatus.MouseY;
            this.MouseLeftDown = AnotherStatus.MouseLeftDown;
            this.MouseLeftUp = AnotherStatus.MouseLeftUp;
            this.MouseRightDown = AnotherStatus.MouseRightDown;
            this.MouseRightUp = AnotherStatus.MouseRightUp;
            this.MouseMove = AnotherStatus.MouseMove;
            this.MouseOverPet = AnotherStatus.MouseOverPet;
        }
    }
}
