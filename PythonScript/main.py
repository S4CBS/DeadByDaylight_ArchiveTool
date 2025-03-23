from support import *
headers = getHeaders()
s,k = ActiveQuest(headers)
All_Quests = CreateNextQuestList(s, k, headers)
# PickNewQuest(s, k, All_Quests, headers)