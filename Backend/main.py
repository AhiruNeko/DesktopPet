import asyncio
import websockets
import json

connected = set()


async def handler(websocket):
    connected.add(websocket)
    try:
        async for message in websocket:
            data = json.loads(message)
            print("Data received：", data)

            # handling data
            # await websocket.send(json.dumps({
            #     "action": "move",
            #     "target": [100, 100]
            # }))
    finally:
        connected.remove(websocket)


async def main():
    async with websockets.serve(handler, "localhost", 8765):
        print("Waiting for connection...")
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())
