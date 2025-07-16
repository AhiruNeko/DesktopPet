import json
import ctypes

from Backend.Mouse import Mouse
from Status import Status
import copy


class Desktop_Pet:

    def __init__(self):
        self.config_path = "../UserPets/" + self.__class__.__name__ + "/config.json"
        with open(self.config_path, "r") as f:
            self.config = json.load(f)
        if self.config["name"] != self.__class__.__name__:
            raise Exception("Inconsistent Naming: \n\tClass Name: " + self.__class__.__name__ +
                            "\n\tName in config.json: " + self.config["name"])
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
            raise Exception("Missing Default")

        self.interactions = []
        methods = []
        for attr_name in dir(self):
            attr = getattr(self, attr_name)
            if callable(attr) and getattr(attr, "_is_interaction", False):
                priority = getattr(attr, "_interaction_priority", 0)
                methods.append((priority, attr))
        methods.sort(key=lambda x: x[0])
        self.interactions = [method for _, method in methods]

        self.mouse_event: Mouse
        self.pre_status = copy.deepcopy(self.status)

        self.dx = 0
        self.dy = 0

    async def send_data(self, send_type="update"):
        import utils
        if self.pre_status == self.status and send_type == "update":
            return True
        self.status.set_type(send_type)
        await utils.send_data(self.status.serialization())
        self.pre_status = copy.deepcopy(self.status)
        return True

    async def execute_interactions(self, mouse_event: Mouse):
        self.mouse_event = mouse_event
        for func in self.interactions:
            if func():
                await self.send_data()
                break
        else:
            self.status.set_path(self.config["default"])
            await self.send_data("update")

    def dragging(self):
        if self.mouse_event.mouse_left_down and self.mouse_event.mouse_over_pet:
            self.dx = self.status.X - self.mouse_event.mouse_x
            self.dy = self.status.Y - self.mouse_event.mouse_y
        if self.mouse_event.mouse_left_pressed and self.mouse_event.mouse_over_pet and self.mouse_event.mouse_move:
            self.status.X = self.dx + self.mouse_event.mouse_x
            self.status.Y = self.dy + self.mouse_event.mouse_y
            return True
        return False

    def left_click(self, path):
        if self.mouse_event.mouse_left_pressed and self.mouse_event.mouse_over_pet:
            self.status.set_path(path)
            return True
        return False
