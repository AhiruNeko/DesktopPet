class Interaction_Result:

    def __init__(self, flag):
        self.flag: str = flag

    def __eq__(self, other):
        if not isinstance(other, Interaction_Result):
            return False
        return self.flag == other.flag

    def __str__(self):
        return self.flag


# Miss condition and continue and not send
PASS = Interaction_Result("PASS")

# Satisfy condition and continue and send
CONTINUE = Interaction_Result("CONTINUE")

# Satisfies condition and break and send
SUCCESS = Interaction_Result("SUCCESS")

# Satisfies condition and break but do not send
FAIL = Interaction_Result("FAIL")
