using BarasToolba;
using CranchyLib.Networking;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;

namespace BarasToolba
{
    public static class Archives
    {
        public struct S_MatchData
        {
            public string matchId;
            public string krakenMatchId;

            public S_MatchData(string matchId, string krakenMatchId)
            {
                this.matchId = matchId;
                this.krakenMatchId = krakenMatchId;
            }
        }
        public struct S_Quest
        {
            public int level;
            public string nodeId;
            public string storyId;
            public int currentProgression;
            public int neededProgression;
            public JArray questEvents;

            public S_Quest(int level, string nodeId, string storyId, int currentProgression, int neededProgression, JArray questEvents)
            {
                this.level = level;
                this.nodeId = nodeId;
                this.storyId = storyId;
                this.currentProgression = currentProgression;
                this.neededProgression = neededProgression;
                this.questEvents = questEvents;
            }
        }




        public static S_MatchData lastSuccessfulMatch;



        private static S_Quest GetActiveQuest()
        {
            List<string> headers = new List<string>()
            {
                $"api-key: {Globals_Session.Game.api_key}",
                $"x-kraken-analytics-session-id: {Globals_Session.Game.client_kraken_session}",
                $"User-Agent: {Globals_Session.Game.user_agent}",
                $"x-kraken-client-platform: {Globals_Session.Game.client_platform}",
                $"x-kraken-client-provider: {Globals_Session.Game.client_provider}",
                $"x-kraken-client-os: {Globals_Session.Game.client_os}",
                $"x-kraken-client-version: {Globals_Session.Game.client_version}",
                "Content-Type: application/json"
            };

            var getActiveNodeResponse = Networking.Get($"https://{Globals_Session.Game.PLT}/api/v1/archives/stories/get/activeNode", headers);
            if (getActiveNodeResponse.statusCode == Networking.E_StatusCode.OK)
            {
                if (getActiveNodeResponse.content.IsJson())
                {
                    JObject activeNodeJson = JObject.Parse(getActiveNodeResponse.content);
                    JToken specificRoleNodeData = null;
                    int currentProgression = 0;
                    int neededProgression = 0;
                    JArray questEvents = new JArray();

                    switch (Globals_Session.Game.playerRole)
                    {
                        case Globals_Session.Game.E_PlayerRole.Survivor:
                            if (activeNodeJson.ContainsKey("survivorActiveNode"))
                            {
                                specificRoleNodeData = activeNodeJson["survivorActiveNode"];
                            }
                            else if (activeNodeJson.ContainsKey("killerActiveNode")){
                                specificRoleNodeData = activeNodeJson["killerActiveNode"];
                                Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.Killer;
                                Form.PlayerRole.Text = "Роль: Killer";
                            }
                            else
                            {
                                return new S_Quest(-1, null, null, -1, -1, null); // Something went wrong, return dummy quest data.
                            }

                            break;

                        case Globals_Session.Game.E_PlayerRole.Killer:
                            if (activeNodeJson.ContainsKey("killerActiveNode"))
                            {
                                specificRoleNodeData = activeNodeJson["killerActiveNode"];
                            }
                            else if (activeNodeJson.ContainsKey("survivorActiveNode"))
                            {
                                specificRoleNodeData = activeNodeJson["survivorActiveNode"];
                                Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.Survivor;
                                Form.PlayerRole.Text = "Роль: Survivor";
                            }
                            else
                            {
                                return new S_Quest(-1, null, null, -1, -1, null); // Something went wrong, return dummy quest data.
                            }

                            break;
                    }

                    foreach (JToken activeNode in activeNodeJson["activeNode"])
                    {
                        JToken activeNodeTreeCoordinate = activeNode["nodeTreeCoordinate"];
                        if ((string)activeNodeTreeCoordinate["nodeId"] == (string)specificRoleNodeData["nodeId"])
                        {
                            JArray activeNodeObjectives = (JArray)activeNode["objectives"];
                            foreach (JToken objective in activeNodeObjectives)
                            {
                                currentProgression = (int)objective["currentProgress"];
                                neededProgression = (int)objective["neededProgression"];

                                JArray objectiveQuestEvents = (JArray)objective["questEvent"];
                                foreach (JToken questEvent in objectiveQuestEvents) // I don't think there's any quest consist of more than 1 quest event, foreach loop is here just in case.
                                {
                                    questEvents.Add(questEvent);
                                }
                            }
                        }
                    }

                    return new S_Quest((int)specificRoleNodeData["level"], (string)specificRoleNodeData["nodeId"], (string)specificRoleNodeData["storyId"], currentProgression, neededProgression, questEvents);
                }
            }

            return new S_Quest(-1, null, null, -1, -1, null); // This return can only be achieved if response status code wasn't 200 (OK) or response wasn't JSON.
        }
        public static void CompleteActiveQuest(S_MatchData matchData)
        {
            Console.WriteLine("[DEBUG] === Начало выполнения CompleteActiveQuest ===");
            Console.WriteLine($"[DEBUG] Match ID: {matchData.matchId}, KrakenMatch ID: {matchData.krakenMatchId}");

            if (matchData.matchId == lastSuccessfulMatch.matchId && matchData.krakenMatchId == lastSuccessfulMatch.krakenMatchId)
            {
                Console.WriteLine("[DEBUG] Этот матч уже был успешно обработан ранее. Пропуск.");
                return;
            }

            Console.WriteLine("[DEBUG] Получаем активный квест...");
            S_Quest activeQuest = GetActiveQuest();

            if (activeQuest.nodeId == null)
            {
                Console.WriteLine("[DEBUG] Не удалось получить активный квест — nodeId отсутствует.");
                return;
            }

            if (activeQuest.currentProgression == activeQuest.neededProgression)
            {
                Console.WriteLine("[DEBUG] Квест уже завершён, ничего не требуется.");
                return;
            }

            Console.WriteLine($"[DEBUG] Активный квест: Уровень={activeQuest.level}, NodeID={activeQuest.nodeId}, StoryID={activeQuest.storyId}");
            Console.WriteLine($"[DEBUG] Прогресс: {activeQuest.currentProgression}/{activeQuest.neededProgression}");
            Console.WriteLine($"[DEBUG] Количество событий квеста: {activeQuest.questEvents.Count}");

            List<string> headers = new List<string>()
    {
        $"api-key: {Globals_Session.Game.api_key}",
        $"x-kraken-analytics-session-id: {Globals_Session.Game.client_kraken_session}",
        $"User-Agent: {Globals_Session.Game.user_agent}",
        $"x-kraken-client-platform: {Globals_Session.Game.client_platform}",
        $"x-kraken-client-provider: {Globals_Session.Game.client_provider}",
        $"x-kraken-client-os: {Globals_Session.Game.client_os}",
        $"x-kraken-client-version: {Globals_Session.Game.client_version}",
        "Content-Type: application/json"
    };

            if (activeQuest.questEvents.Count == 1)
            {
                Console.WriteLine("[DEBUG] Квест состоит из одного события. Корректируем количество повторений...");
                if ((int)activeQuest.questEvents[0]["repetition"] <= activeQuest.neededProgression)
                {
                    int questProgressionToComplete = activeQuest.neededProgression - activeQuest.currentProgression;
                    Console.WriteLine($"[DEBUG] Прогресс для завершения: {questProgressionToComplete}");
                    activeQuest.questEvents[0]["repetition"] = questProgressionToComplete;
                }
            }

            JObject requestBodyJson = JObject.FromObject(new
            {
                matchId = matchData.matchId,
                krakenMatchId = matchData.krakenMatchId,
                questEvents = activeQuest.questEvents,
                role = Globals_Session.Game.playerRole.ToString().ToLower()
            });

            Console.WriteLine("[DEBUG] Отправляем запрос на выполнение квеста...");
            var updateQuestProgressResponse = Networking.Post(
                $"https://{Globals_Session.Game.PLT}/api/v1/archives/stories/update/quest-progress-v3/",
                headers,
                requestBodyJson.ToString()
            );

            Console.WriteLine($"[DEBUG] Статус ответа: {updateQuestProgressResponse.statusCode}");
            Console.WriteLine($"[DEBUG] Тело ответа: {updateQuestProgressResponse.content}");

            Form.PlayerRole.Text = "Роль: xxx";
            Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.None;
            Media.playSound();

            if (updateQuestProgressResponse.statusCode == Networking.E_StatusCode.CONTINUE)
            {
                Console.WriteLine("[DEBUG] Квест успешно обновлён! Обновляем lastSuccessfulMatch.");
                lastSuccessfulMatch = matchData;
            }
            else
            {
                Console.WriteLine("[DEBUG] Ответ не содержит статус CONTINUE — выполнение не подтверждено.");
            }

            Console.WriteLine("[DEBUG] Запуск PickNewQuestAsync...");
            Program.PickNewQuestAsync();

            Console.WriteLine("[DEBUG] Обновление данных FiddlerCore...");
            FiddlerCore.UpdateData();

            Console.WriteLine("[DEBUG] === Завершено выполнение CompleteActiveQuest ===");
        }
    }
}
