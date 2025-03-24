from support import *
headers = getHeaders()
s_id, k_id = ActiveQuest(headers)
all_quests = CreateNextQuestList(s_id, k_id, headers)
PickNewQuest(s_id, k_id, all_quests, headers)