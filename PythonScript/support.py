import time

from importS import *

# Paths
pathToappdata = os.getenv("APPDATA")
pathTocfg = os.path.join(pathToappdata, "BarasToolba", "cfg.json")

def getHeaders():
    with open(pathTocfg, "r") as file:
        js = json.load(file)

    bhvr = "bhvrSession=" + js["bhvrSession"]
    x_kraken_analytics_session_id = js["x_kraken_analytics_session_id"]
    useragent = js["useragent"]
    platform = js["platform"]
    provider = js["provider"]
    client_os = js["os"]
    version = js["version"]

    headers = {
        "Cookie": bhvr,
        "User-Agent": useragent,
        "x-kraken-client-platform": platform,
        "x-kraken-client-provider": provider,
        "x-kraken-client-os": client_os,
        "x-kraken-analytics-session-id": x_kraken_analytics_session_id,
        "x-kraken-client-version": version
    }

    return headers

def ActiveQuest(headers):
    url = "https://egs.live.bhvrdbd.com/api/v1/archives/stories/get/activeNode"
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


def CreateNextQuestList(s, k, headers):
    xs = []
    Tomes = ['Tome22', 'Tome21', 'Tome20', 'Tome19', 'Tome18', 'Tome17', 'Tome16', 'Tome15', 'Tome14', 'Tome13',
             'Tome12', 'Tome11', 'Tome10', 'Tome09', 'Tome08', 'Tome07', 'Tome06', 'Tome05', 'Tome04', 'Tome03',
             'Tome02', 'Tome01']

    if k != 0:
        try:
            Tomes.remove(k["storyId"])
            Tomes.insert(0, k["storyId"])
        except:
            pass
    if s != 0:
        try:
            Tomes.remove(s["storyId"])
            Tomes.insert(0, s["storyId"])
        except:
            pass

    def process_tome(tome, headers, k, s):
        th = threading.Thread(target=time.sleep(3), daemon=True)
        th.start()
        th.join()
        url = f"https://egs.live.bhvrdbd.com/api/v1/archives/stories/get/story?storyId={tome}"
        resp = requests.get(url=url, headers=headers, verify=False)
        Rjson = resp.json()["listOfNodes"]
        nodes = []
        for node in Rjson:
            if node["status"] == "open":
                node_coord = {
                    "level": node["nodeTreeCoordinate"]["level"],
                    "nodeId": node["nodeTreeCoordinate"]["nodeId"],
                    "storyId": node["nodeTreeCoordinate"]["storyId"]
                }
                if node_coord != k and node_coord != s:
                    nodes.append(node_coord)
        return nodes

    # Запускаем параллельные запросы с сохранением порядка Tomes
    with ThreadPoolExecutor(max_workers=12) as executor:
        process_func = partial(process_tome, headers=headers, k=k, s=s)
        results = executor.map(process_func, Tomes)

        for result in results:
            xs.extend(result)

    return xs

def PickNewQuest(s, k, All_Quests, headers):
    if s != 0 and k != 0:
        return
    if s == 0:
        for x in All_Quests:
            jsonBody = {
                "node": x,
                "role": "survivor"
            }

            url = "https://egs.live.bhvrdbd.com/api/v1/archives/stories/update/active-node-v3"

            resp = requests.post(url=url, headers=headers, verify=False, json=jsonBody)

            if resp.status_code == 200:
                return

    if k == 0:
        for x in All_Quests:
            jsonBody = {
                "node": x,
                "role": "killer"
            }

            url = "https://egs.live.bhvrdbd.com/api/v1/archives/stories/update/active-node-v3"

            resp = requests.post(url=url, headers=headers, verify=False, json=jsonBody)

            if resp.status_code == 200:
                return
