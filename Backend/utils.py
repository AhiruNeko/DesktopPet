import json
import websockets
from Desktop_Pet import Desktop_Pet
import os
import importlib.util
from Backend.registry import REGISTRY

connected = set()
DATA = {}
PET: Desktop_Pet = None


async def receive_data(websocket):
    global DATA
    load_user_pet("../UserPets")
    connected.add(websocket)
    set_pet()

    try:
        await PET.send_data("init")
        print(f"Init message sent to {websocket.remote_address}")

        async for message in websocket:
            DATA = json.loads(message)
            print("Receiving：", DATA)

            # handling data
            # await websocket.send(json.dumps({
            #     "action": "move",
            #     "target": [100, 100]
            # }))
    except websockets.ConnectionClosedOK:
        print("Connection closed normally.")
    except websockets.ConnectionClosedError as e:
        print(f"Connection closed with error: {e}")
    except Exception as e:
        print(f"Unexpected error: {e}")
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
        raise "Do Not Have a Selected Pet."


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
