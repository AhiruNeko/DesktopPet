import asyncio
import websockets
import utils


async def main():
    async with websockets.serve(utils.handler, "localhost", 8765):
        print("Waiting for connection...")
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())
