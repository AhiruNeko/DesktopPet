import json
import ctypes
from Status import Status


class Desktop_Pet:

    def __init__(self):
        self.config_path = "../UserPets/" + self.__class__.__name__ + "/config.json"
        with open(self.config_path, "r") as f:
            self.config = json.load(f)
        if self.config["name"] != self.__class__.__name__:
            raise "Inconsistent Naming: \n\tClass Name: " + self.__class__.__name__ \
                  + "\n\tName in config.json: " + self.config["name"]
        self.name = self.config["name"]

        user32 = ctypes.windll.user32
        user32.SetProcessDPIAware()
        screen_width = user32.GetSystemMetrics(0)
        screen_height = user32.GetSystemMetrics(1)

        self.status = Status(self)
        self.status.set_x(self.config["x"] if "x" in self.config else screen_width - 350)
        self.status.set_y(self.config["y"] if "y" in self.config else screen_height - 400)
        self.status.set_width(self.config["width"] if "width" in self.config else 200)
        self.status.set_height(self.config["height"] if "height" in self.config else 200)
        if "default" in self.config:
            self.status.set_path(self.config["default"])
        else:
            raise "Missing Default"

    async def send_data(self, send_type):
        import utils
        self.status.set_type(send_type)
        await utils.send_data(self.status.serialization())



