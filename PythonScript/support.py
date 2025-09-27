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

    survivor = 0
    killer = 0

    if Rjson.get("survivorActiveNode"):
        refactroring = {
            "level": Rjson["survivorActiveNode"]["level"],
            "nodeId": Rjson["survivorActiveNode"]["nodeId"],
            "storyId": Rjson["survivorActiveNode"]["storyId"]
        }
        survivor = refactroring
    else:
        survivor = 0

    if Rjson.get("killerActiveNode"):
        refactroring = {
            "level": Rjson["killerActiveNode"]["level"],
            "nodeId": Rjson["killerActiveNode"]["nodeId"],
            "storyId": Rjson["killerActiveNode"]["storyId"]
        }
        killer = refactroring
    else:
        killer = 0

    return survivor, killer


def CreateNextQuestList(s, k, headers, host):
    xs = []
    Tomes = getTomeList()[::-1]

    for Tome in Tomes:
        url = f"https://{host}/api/v1/archives/stories/get/story?storyId={Tome}"
        resp = requests.get(url=url, headers=headers, verify=False)
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

def PickNewQuest(s, k, All_Quests, headers, host):
    if s != 0 and k != 0:
        return
    if s == 0:
        for x in All_Quests:
            jsonBody = {
                "node": x,
                "role": "survivor"
            }

            url = f"https://{host}/api/v1/archives/stories/update/active-node-v3"

            resp = requests.post(url=url, headers=headers, verify=False, json=jsonBody)

            if resp.status_code == 200:
                return

    if k == 0:
        for x in All_Quests:
            jsonBody = {
                "node": x,
                "role": "killer"
            }

            url = f"https://{host}/api/v1/archives/stories/update/active-node-v3"

            resp = requests.post(url=url, headers=headers, verify=False, json=jsonBody)

            if resp.status_code == 200:
                return
