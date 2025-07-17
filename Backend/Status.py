import os, copy


class Status:

    def __init__(self, pet):
        self.pet = pet
        self.Type = "update"
        self.Path = ""
        self.SoundPath = ""
        self.PlaySound = False
        self.Width = 0
        self.Height = 0
        self.X = 0
        self.Y = 0

    def __eq__(self, other):
        if not isinstance(other, Status):
            return False
        return (
            self.Type == other.Type and
            self.Path == other.Path and
            self.SoundPath == other.SoundPath and
            self.PlaySound == other.PlaySound and
            self.Width == other.Width and
            self.Height == other.Height and
            self.X == other.X and
            self.Y == other.Y
        )

    def __deepcopy__(self, memo):
        cls = self.__class__
        result = cls.__new__(cls)
        memo[id(self)] = result

        for k, v in self.__dict__.items():
            if k == "pet":
                setattr(result, k, v)
            else:
                setattr(result, k, copy.deepcopy(v, memo))
        return result

    def serialization(self):
        return {
            "Type": self.Type,
            "Path": self.Path,
            "SoundPath": self.SoundPath,
            "PlaySound": self.PlaySound,
            "Width": self.Width,
            "Height": self.Height,
            "X": self.X,
            "Y": self.Y
        }

    def set_path(self, path: str):
        relative_path = "../UserPets/" + self.pet.name + "/assets/" + path
        self.Path = os.path.abspath(relative_path)

    def set_sound_path(self, path: str):
        relative_path = "../UserPets/" + self.pet.name + "/assets/" + path
        self.SoundPath = os.path.abspath(relative_path)

    def set_play_sound(self, play_sound: bool):
        self.PlaySound = play_sound

    def set_type(self, send_type: str):
        self.Type = send_type

    def set_width(self, width: float):
        self.Width = width

    def set_height(self, height):
        self.Height = height

    def set_x(self, x):
        self.X = x

    def set_y(self, y):
        self.Y = y

