from Backend.Desktop_Pet import Desktop_Pet
from Backend.registry import register
from Backend.utils import interaction


@register
class Demo_Pet(Desktop_Pet):

    def __init__(self):
        super().__init__()
        self.dx = 0
        self.dy = 0

    @interaction
    async def dragging(self):
        if self.mouse_event.mouse_left_down and self.mouse_event.mouse_over_pet:
            self.dx = self.status.X - self.mouse_event.mouse_x
            self.dy = self.status.Y - self.mouse_event.mouse_y
        if self.mouse_event.mouse_left_pressed and self.mouse_event.mouse_over_pet:
            self.status.set_path("2.png")
            if self.mouse_event.mouse_move:
                self.status.X = self.dx + self.mouse_event.mouse_x
                self.status.Y = self.dy + self.mouse_event.mouse_y
            return await self.send_data()
        return False
