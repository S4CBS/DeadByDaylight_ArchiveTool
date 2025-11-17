from support import *
import asyncio

async def main():
    try:
        headers, host = getHeaders()
        await asyncio.sleep(1)  # задержка 1 секунды

        s_id, s, k_id, k, s_c, k_c = ActiveQuest(headers, host)
        await asyncio.sleep(1)  # задержка 1 секунды

        DeactiveQuest(headers, host, s_id, s, k_id, k, s_c, k_c)
        await asyncio.sleep(1)  # задержка 1 секунды

        s_id, s, k_id, k, s_c, k_c = ActiveQuest(headers, host)
        await asyncio.sleep(1)  # задержка 1 секунды

        all_quests = CreateNextQuestList(s_id, k_id, headers, host)
        await asyncio.sleep(1)  # задержка 1 секунды

        PickNewQuest(all_quests, headers, host, s, k)
    except Exception:
        pass

# Запуск асинхронной функции
asyncio.run(main())