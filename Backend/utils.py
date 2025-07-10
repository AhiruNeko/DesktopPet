import json
import websockets

connected = set()
DATA = {}


async def handler(websocket):
    global DATA
    connected.add(websocket)
    try:
        async for message in websocket:
            DATA = json.loads(message)
            print("Data received：", DATA)

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
