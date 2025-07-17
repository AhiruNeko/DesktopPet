import functools
import json
import traceback

import websockets

from Backend import Interaction_Result
from Desktop_Pet import Desktop_Pet
import os
import importlib.util
from Backend.registry import REGISTRY
from Mouse import Mouse

connected = set()
DATA = {}
PET: Desktop_Pet
MOUSE: Mouse = Mouse()


async def receive_data(websocket):
    global DATA, MOUSE
    load_user_pet("../UserPets")
    connected.add(websocket)
    set_pet()

    try:
        await PET.send_data("init")
        print(f"Init message sent to {websocket.remote_address}")

        async for message in websocket:
            DATA = json.loads(message)
            print("Receiving：", DATA)
            MOUSE.update_mouse(DATA)
            await PET.execute_interactions(MOUSE, DATA)

    except websockets.ConnectionClosedOK:
        print("Connection closed normally.")
    except websockets.ConnectionClosedError as e:
        print(f"Connection closed with error: {e}")
    except Exception as e:
        print(f"Unexpected error: {e}")
        traceback.print_exc()
    finally:
        print("Client disconnected.")


async def send_data(data: dict):
    if not connected:
        print("No clients connected.")
        return

    message = json.dumps(data)
    disconnected = set()

    for ws in connected:
        try:
            await ws.send(message)
            print("Sending: " + str(data))
        except Exception as e:
            print(f"Send failed: {e}")
            disconnected.add(ws)

    connected.difference_update(disconnected)


def set_pet():
    global PET
    with open("../settings.json", "r") as f:
        settings = json.load(f)
    if "using" in settings:
        PET = REGISTRY[settings["using"]]()
    else:
        raise Exception("Do Not Have a Selected Pet.")


def load_user_pet(plugin_dir):
    for root, _, files in os.walk(plugin_dir):
        for file in files:
            if file.endswith(".py") and file != "__init__.py":
                file_path = os.path.join(root, file)
                module_name = os.path.splitext(os.path.relpath(file_path, plugin_dir))[0].replace(os.sep, ".")

                spec = importlib.util.spec_from_file_location(module_name, file_path)
                if spec and spec.loader:
                    module = importlib.util.module_from_spec(spec)
                    spec.loader.exec_module(module)


def interaction(priority=0):
    def decorator(func):
        func._is_interaction = True
        func._interaction_priority = priority
        return func
    return decorator


def monitor(func):
    func._is_monitor = True
    return func

