import json
import websockets

connected = set()
DATA = {}


async def receive_data(websocket):
    global DATA
    connected.add(websocket)

    init_data = {
        "Type": "init",
        "X": 0,
        "Y": 0,
        "Width": 200,
        "Height": 200,
        "Path": "D:/OKC/projects/codes/Others/DesktopPet/UserPets\Demo_Pet/assets/1.png"
    }

    try:
        await websocket.send(json.dumps(init_data))
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
