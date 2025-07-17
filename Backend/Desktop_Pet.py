import json
import ctypes
import copy
import inspect
import asyncio
from collections import deque

from Backend.Mouse import Mouse
from Status import Status
import Interaction_Result
from typing import Callable, Awaitable, Optional


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
        self.screen_width = user32.GetSystemMetrics(0)
        self.screen_height = user32.GetSystemMetrics(1)

        self.status = Status(self)
        self.status.set_x(self.config["x"] if "x" in self.config else self.screen_width - 350)
        self.status.set_y(self.config["y"] if "y" in self.config else self.screen_height - 350)
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

        self.monitor = None
        for attr_name in dir(self):
            attr = getattr(self, attr_name)
            if callable(attr) and getattr(attr, "_is_monitor", False):
                self.monitor = attr

        self.mouse_event: Mouse
        self.pre_status = copy.deepcopy(self.status)

        self._motion_task: Optional[asyncio.Task] = None
        self._motion_priority: int = 0
        self.frame_timer = 0
        self.animation_fps = 30
        self.frame_interval = 1 / self.animation_fps
        self._animation_loop_task = asyncio.create_task(self._animation_loop())

        self._send_lock = asyncio.Lock()

        self.dx = 0
        self.dy = 0
        self._position_history = deque(maxlen=5)

    def get_screen_size(self):
        return self.screen_width, self.screen_height

    async def send_data(self, send_type="update"):
        import utils
        async with self._send_lock:
            if self.pre_status == self.status and send_type == "update":
                return
            self.status.set_type(send_type)
            await utils.send_data(self.status.serialization())
            self.pre_status = copy.deepcopy(self.status)
            self.status.set_play_sound(False)

    async def execute_interactions(self, mouse_event: Mouse, recv_data):
        self.mouse_event = mouse_event
        anime_cancel = False
        if recv_data["Type"] == "update":
            for func in self.interactions:

                priority = getattr(func, "_interaction_priority", 0)
                if self._motion_task and not self._motion_task.done():
                    if self._motion_priority <= priority:
                        continue

                result_or_coro = func()
                if inspect.isawaitable(result_or_coro):
                    result = await result_or_coro
                else:
                    result = result_or_coro

                if result == Interaction_Result.SUCCESS:
                    if self._motion_task and not self._motion_task.done():
                        self._motion_task.cancel()
                        anime_cancel = True
                    break
                elif result == Interaction_Result.PASS:
                    continue
                elif result == Interaction_Result.FAIL:
                    break
                elif result == Interaction_Result.CONTINUE:
                    if self._motion_task and not self._motion_task.done():
                        self._motion_task.cancel()
                        anime_cancel = True
                    continue
            else:
                if not self._motion_task or self._motion_task.done():
                    self.status.set_path(self.config["default"])
        elif recv_data["Type"] == "reset":
            self.status.set_path(recv_data["Path"])
            self.status.set_x(recv_data["X"])
            self.status.set_y(recv_data["Y"])
            self.status.set_width(recv_data["Width"])
            self.status.set_height(recv_data["Height"])
            self.status.set_sound_path(recv_data["SoundPath"])
            self.status.set_play_sound(recv_data["PlaySound"])

        if not self._motion_task or self._motion_task.done() or anime_cancel:
            await self.send_data("update")

    def play_sound(self, path: str):
        self.status.set_play_sound(True)
        self.status.set_sound_path(path)

    def get_velocity(self):
        if len(self._position_history) < 2:
            return 0.0, 0.0

        dx = self._position_history[-1][0] - self._position_history[0][0]
        dy = self._position_history[-1][1] - self._position_history[0][1]
        frame_count = len(self._position_history) - 1

        return dx / frame_count, dy / frame_count

    def start_motion(self, coro_fn: Callable[[], Awaitable], priority: int):
        if self._motion_task and not self._motion_task.done():
            if self._motion_priority <= priority:
                return
            else:
                self._motion_task.cancel()

        self._motion_priority = priority
        self._motion_task = asyncio.create_task(self._wrap_motion(coro_fn))

    async def _wrap_motion(self, coro_fn: Callable[[], Awaitable]):
        try:
            await coro_fn()
        except asyncio.CancelledError:
            pass
        finally:
            self._motion_task = None
            self._motion_priority = 0

    async def _animation_loop(self):
        while True:
            self.frame_timer += 1
            self._position_history.append((self.status.X, self.status.Y))
            if self.monitor:
                if inspect.isawaitable(self.monitor):
                    await self.monitor()
                else:
                    self.monitor()
            await self.send_data("update")
            await asyncio.sleep(self.frame_interval)

    async def wait_frames(self, frame_count: int):
        await asyncio.sleep(frame_count * self.frame_interval)

    def seconds_to_frames(self, seconds: float) -> int:
        return int(seconds * self.animation_fps)

    def frames_to_seconds(self, frames: int) -> float:
        return frames / self.animation_fps

    def dragging(self, decay=0.8, min_v=0.5, anime_priority=0) -> Interaction_Result:
        if self.mouse_event.mouse_left_down and self.mouse_event.mouse_over_pet:
            self.dx = self.status.X - self.mouse_event.mouse_x
            self.dy = self.status.Y - self.mouse_event.mouse_y
        if self.mouse_event.mouse_left_pressed and self.mouse_event.mouse_over_pet and self.mouse_event.mouse_move:
            self.status.X = self.dx + self.mouse_event.mouse_x
            self.status.Y = self.dy + self.mouse_event.mouse_y
            return Interaction_Result.SUCCESS
        return Interaction_Result.PASS

    def left_pressed(self, path) -> Interaction_Result:
        if self.mouse_event.mouse_left_pressed and self.mouse_event.mouse_over_pet:
            self.status.set_path(path)
            return Interaction_Result.SUCCESS
        return Interaction_Result.PASS

    def right_pressed(self, path) -> Interaction_Result:
        if self.mouse_event.mouse_right_pressed and self.mouse_event.mouse_over_pet:
            self.status.set_path(path)
            return Interaction_Result.SUCCESS
        return Interaction_Result.PASS
