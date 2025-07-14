from Backend.Desktop_Pet import Desktop_Pet
from Backend.registry import register


@register
class Demo_Pet(Desktop_Pet):

    def __init__(self):
        super().__init__()
