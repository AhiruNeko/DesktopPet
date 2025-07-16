from Backend.Desktop_Pet import Desktop_Pet
from Backend.registry import register
from Backend.utils import interaction
import asyncio
from Backend import Interaction_Result


@register
class Demo_Pet(Desktop_Pet):

    def __init__(self):
        super().__init__()

    @interaction(priority=0)
    def left_click(self):
        return super().left_pressed("2.png")

    @interaction(priority=1)
    async def move(self):
        if self.mouse_event.mouse_right_pressed and self.mouse_event.mouse_over_pet:
            self.start_motion(self._move_animation, priority=1)
            return Interaction_Result.CONTINUE
        return Interaction_Result.PASS

    async def _move_animation(self):
        for _ in range(30):
            self.status.X -= 15
            await self.wait_frames(1)

