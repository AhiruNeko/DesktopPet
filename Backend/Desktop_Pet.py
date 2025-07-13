import json
import ctypes
from Status import Status
import utils


class Desktop_Pet:

    def __init__(self):
        self.config_path = "../UserPets/" + self.__class__.__name__ + "/config.json"
        with open(self.config_path, "r") as f:
            self.config = json.load(f)
        if self.config["name"] != self.__class__.__name__:
            raise "Inconsistent Naming: \n\tClass Name: " + self.__class__.__name__ \
                  + "\n\tName in config.json: " + self.config["name"]

        user32 = ctypes.windll.user32
        user32.SetProcessDPIAware()
        screen_width = user32.GetSystemMetrics(0)
        screen_height = user32.GetSystemMetrics(1)

        self.status = Status()
        self.status.X = self.config["x"] if "x" in self.config else screen_width - 250
        self.status.Y = self.config["y"] if "y" in self.config else screen_height - 250
        self.status.Width = self.config["width"] if "width" in self.config else 200
        self.status.Height = self.config["height"] if "height" in self.config else 200
        self.status.Path = self.config["default"]

    async def send_data(self, send_type):
        self.status.Type = send_type
        await utils.send_data(self.status.serialization())


