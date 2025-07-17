from Backend.Desktop_Pet import Desktop_Pet
from Backend.registry import register
from Backend.utils import interaction, monitor
from Backend import Interaction_Result


@register
class Demo_Pet(Desktop_Pet):

    def __init__(self):
        super().__init__()

    @monitor
    def pet_monitor(self):
        vec_x, vec_y = self.get_velocity()
        if (vec_x ** 2) + (vec_y ** 2) >= 25:
            self.status.set_path("5.gif")
        elif (vec_x ** 2) + (vec_y ** 2) < 25:
            self.status.set_path("4.gif")

    @interaction(priority=0)
    def dragging(self):
        if self.mouse_event.mouse_left_up and self.mouse_event.mouse_over_pet:
            self.start_motion(self.anime, priority=1)
            return Interaction_Result.SUCCESS
        else:
            return super().dragging()

    async def anime(self):
        vec_x, vec_y = self.get_velocity()
        decay = 1.1
        min_v = 0.5

        while abs(vec_x) >= min_v or abs(vec_y) >= min_v:
            if self.status.X <= 0:
                self.status.X = 0
                vec_x = -vec_x
            elif self.status.X + self.status.Width >= self.screen_width:
                self.status.X = self.screen_width - self.status.Width
                vec_x = -vec_x

            if self.status.Y <= 0:
                self.status.Y = 0
                vec_y = -vec_y
            elif self.status.Y + self.status.Height >= self.screen_height:
                self.status.Y = self.screen_height - self.status.Height
                vec_y = -vec_y

            self.status.X += vec_x
            self.status.Y += vec_y

            vec_x /= decay
            vec_y /= decay

            await self.wait_frames(1)
