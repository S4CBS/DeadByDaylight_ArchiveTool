from support import *
import asyncio

async def main():
    try:
        headers, host = getHeaders()
        await asyncio.sleep(1)  # задержка 1 секунды

        s_id, k_id = ActiveQuest(headers, host)
        await asyncio.sleep(1)  # задержка 1 секунды

        all_quests = CreateNextQuestList(s_id, k_id, headers, host)
        await asyncio.sleep(1)  # задержка 1 секунды

        PickNewQuest(s_id, k_id, all_quests, headers, host)
    except Exception:
        pass

# Запуск асинхронной функции
asyncio.run(main())