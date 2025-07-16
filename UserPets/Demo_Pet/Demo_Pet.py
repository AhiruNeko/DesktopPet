from Backend.Desktop_Pet import Desktop_Pet
from Backend.registry import register
from Backend.utils import interaction
from collections import deque
from Backend import Interaction_Result


@register
class Demo_Pet(Desktop_Pet):

    def __init__(self):
        super().__init__()

    @interaction(priority=0)
    def dragging(self):
        if self.mouse_event.mouse_left_up and self.mouse_event.mouse_over_pet:
            self.start_motion(self.anime, priority=1)
            return Interaction_Result.SUCCESS
        else:
            return super().dragging()

    async def anime(self):
        vec_x, vec_y = self.get_velocity()
        while abs(vec_x) >= 0.5 or abs(vec_y) >= 0.5:
            self.status.X += vec_x
            self.status.Y += vec_y
            vec_x /= 1.2
            vec_y /= 1.2
            await self.wait_frames(1)
