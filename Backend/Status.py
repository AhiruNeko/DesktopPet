class Status:

    def __init__(self):
        self.Type = "update"
        self.Path = ""
        self.Width = 0
        self.Height = 0
        self.X = 0
        self.Y = 0

    def serialization(self):
        return {
            "Type": self.Type,
            "Path": self.Path,
            "Width": self.Width,
            "Height": self.Height,
            "X": self.X,
            "Y": self.Y
        }
