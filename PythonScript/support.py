import time

from importS import *

# Paths
pathToappdata = os.getenv("APPDATA")
pathTocfg = os.path.join(pathToappdata, "BarasToolba", "cfg.json")
pathToTomesList = os.path.join(pathToappdata, "BarasToolba", "tome.txt")

def getTomeList():
    with open(pathToTomesList, "r") as file:
        f = file.readlines()
    xs = [x.strip() for x in f]
    return xs

def getHeaders():
    with open(pathTocfg, "r") as file:
        js = json.load(file)

    api_key = js["api_key"]
    x_kraken_analytics_session_id = js["x_kraken_analytics_session_id"]
    useragent = js["useragent"]
    platform = js["platform"]
    provider = js["provider"]
    client_os = js["os"]
    version = js["version"]

    host = ""

    headers = {
        "api-key": api_key,
        "User-Agent": useragent,
        "x-kraken-client-platform": platform,
        "x-kraken-client-provider": provider,
        "x-kraken-client-os": client_os,
        "x-kraken-analytics-session-id": x_kraken_analytics_session_id,
        "x-kraken-client-version": version
    }

    if platform == "steam":
        host = "steam.live.bhvrdbd.com"
    elif platform == "egs":
        host = "egs.live.bhvrdbd.com"
    elif platform == "grdk":
        host = "grdk.live.bhvrdbd.com"

    return headers, host

def ActiveQuest(headers, host):
    url = f"https://{host}/api/v1/archives/stories/get/activeNode"
    resp = requests.get(url=url, headers=headers, verify=False)

    Rjson = resp.json()
    print(f"4:\n{Rjson}")

    survivor = 0
    s_c = False # Квест есть

    killer = 0
    k_c = False # Квест есть

    if Rjson.get("survivorClaimableActiveNode") is not None:
        refactroring = {
            "level": Rjson["survivorClaimableActiveNode"]["level"],
            "nodeId": Rjson["survivorClaimableActiveNode"]["nodeId"],
            "storyId": Rjson["survivorClaimableActiveNode"]["storyId"]
        }
        survivor = refactroring
        CompleteQuest(survivor, "survivor", headers, host)

        s_c = True

    elif Rjson.get("survivorActiveNode") is not None:
        refactroring = {
            "level": Rjson["survivorActiveNode"]["level"],
            "nodeId": Rjson["survivorActiveNode"]["nodeId"],
            "storyId": Rjson["survivorActiveNode"]["storyId"]
        }
        survivor = refactroring

    else:
        s_c = True

    if Rjson.get("killerClaimableActiveNode") is not None:
        refactroring = {
            "level": Rjson["killerClaimableActiveNode"]["level"],
            "nodeId": Rjson["killerClaimableActiveNode"]["nodeId"],
            "storyId": Rjson["killerClaimableActiveNode"]["storyId"]
        }
        killer = refactroring
        CompleteQuest(killer, "killer", headers, host)

        k_c = True # Нет квеста вообще

    elif Rjson.get("killerActiveNode") is not None:
        refactroring = {
            "level": Rjson["killerActiveNode"]["level"],
            "nodeId": Rjson["killerActiveNode"]["nodeId"],
            "storyId": Rjson["killerActiveNode"]["storyId"]
        }
        killer = refactroring
    else:
        k_c = True # Нет квеста вообще

    print(survivor, killer, s_c, k_c)
    return survivor, killer, s_c, k_c

def CompleteQuest(questData, role, h, host):
    jsonBody = {
        "node": questData,
        "role": role
    }

    url = f"https://{host}/api/v1/archives/stories/update/active-node-v3"

    resp = requests.post(url=url, headers=h, verify=False, json=jsonBody)
    print(f"Quest Complete:\n{resp.json()}")

def ReactiveQuest(h, host, survivor, killer, s_c, k_c):
    status = False
    if not s_c: # Квест есть
        jsonBody = {
            "node": survivor,
            "role": "survivor"
        }

        url = f"https://{host}/api/v1/archives/stories/update/active-node-v3"

        resp = requests.post(url=url, headers=h, verify=False, json=jsonBody)
        print(f"Deactivate:\n{resp.json()}")

        resp = requests.post(url=url, headers=h, verify=False, json=jsonBody)
        print(f"Activate:\n{resp.json()}")

        if resp.status_code == 200:
            status = True
            return status

    if not k_c: # Квест есть
        jsonBody = {
            "node": killer,
            "role": "killer"
        }

        url = f"https://{host}/api/v1/archives/stories/update/active-node-v3"

        resp = requests.post(url=url, headers=h, verify=False, json=jsonBody)
        print(f"Deactivate:\n{resp.json()}")

        resp = requests.post(url=url, headers=h, verify=False, json=jsonBody)
        print(f"Activate:\n{resp.json()}")

        if resp.status_code == 200:
            status = True
            return status
    return status

def CreateNextQuestList(s, k, headers, host):
    xs = []
    Tomes = getTomeList()[::-1]

    for Tome in Tomes:
        url = f"https://{host}/api/v1/archives/stories/get/story?storyId={Tome}"
        resp = requests.get(url=url, headers=headers, verify=False)
        print(f"3:\n{resp.json()}")
        Rjson = resp.json()["listOfNodes"]
        for node in Rjson:
            if node["status"] == "open":
                node = {
                    "level": node["nodeTreeCoordinate"]["level"],
                    "nodeId": node["nodeTreeCoordinate"]["nodeId"],
                    "storyId": node["nodeTreeCoordinate"]["storyId"]
                }
                if node != k and node != s:
                    xs.append(node)
    return xs

def PickNewQuest(All_Quests, headers, host, su, ki):
    if su:
        for x in All_Quests:
            jsonBody = {
                "node": x,
                "role": "survivor"
            }

            url = f"https://{host}/api/v1/archives/stories/update/active-node-v3"

            resp = requests.post(url=url, headers=headers, verify=False, json=jsonBody)
            print(f"1:\n{resp.json()}")

            if resp.status_code == 200:
                return

    if ki:
        for x in All_Quests:
            jsonBody = {
                "node": x,
                "role": "killer"
            }

            url = f"https://{host}/api/v1/archives/stories/update/active-node-v3"

            resp = requests.post(url=url, headers=headers, verify=False, json=jsonBody)
            print(f"2:\n{resp.json()}")

            if resp.status_code == 200:
                return