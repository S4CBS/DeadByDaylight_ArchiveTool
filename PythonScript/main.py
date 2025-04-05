from support import *
try:
    headers, host = getHeaders()
    s_id, k_id = ActiveQuest(headers, host)
    all_quests = CreateNextQuestList(s_id, k_id, headers, host)
    PickNewQuest(s_id, k_id, all_quests, headers, host)
except:
    pass
