import json

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
    finally:
        connected.remove(websocket)