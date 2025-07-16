from Backend.Desktop_Pet import Desktop_Pet
from Backend.registry import register
from Backend.utils import interaction


@register
class Demo_Pet(Desktop_Pet):

    def __init__(self):
        super().__init__()

    @interaction(priority=1)
    def dragging(self):
        return super().dragging()

    @interaction(priority=0)
    def left_click(self):
        return super().left_click("2.png")
