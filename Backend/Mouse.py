class Mouse:

    def __init__(self):
        self.mouse_x = 0
        self.mouse_y = 0
        self.mouse_left_down = False
        self.mouse_left_up = False
        self.mouse_right_down = False
        self.mouse_right_up = False
        self.mouse_left_pressed = False
        self.mouse_right_pressed = False
        self.mouse_move = False
        self.mouse_over_pet = False

    def update_mouse(self, data: dict):
        self.mouse_x = data["MouseX"]
        self.mouse_y = data["MouseY"]
        self.mouse_left_down = data["MouseLeftDown"]
        self.mouse_left_up = data["MouseLeftUp"]
        self.mouse_right_down = data["MouseRightDown"]
        self.mouse_right_up = data["MouseRightUp"]
        self.mouse_left_pressed = data["MouseLeftPressed"]
        self.mouse_right_pressed = data["MouseRightPressed"]
        self.mouse_move = data["MouseMove"]
        self.mouse_over_pet = data["MouseOverPet"]

    @property
    def no_event(self):
        return not any([
            self.mouse_left_down,
            self.mouse_left_up,
            self.mouse_right_down,
            self.mouse_right_up,
            self.mouse_left_pressed,
            self.mouse_right_pressed,
            self.mouse_move,
            self.mouse_over_pet
        ])
