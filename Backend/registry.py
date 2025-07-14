REGISTRY = {}


def register(cls):
    global REGISTRY
    REGISTRY[cls.__name__] = cls
    return cls
