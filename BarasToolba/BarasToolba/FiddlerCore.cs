using System.Data;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using BarasToolba;
using CranchyLib.Networking;
using Fiddler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarasToolba
{

    public static class Globals_Session
    {
        public static class Game
        {
            public enum E_PlayerRole
            {
                None,
                Survivor,
                Killer
            }
            public enum E_MatchType
            {
                None,
                Default,
                Custom
            }


            public static JToken survivor = null;
            public static JToken killer = null;

            public static string bhvrSession = null;
            public static string userId = null;

            public static string PLT = null;

            public static bool isInQueue = false;
            public static bool isInMatch = false;


            public static string matchId = null;
            public static E_MatchType matchType = E_MatchType.None;
            public static E_PlayerRole playerRole = E_PlayerRole.None;

            public static string user_agent = null;
            public static string client_version = null;
            public static string client_provider = null;
            public static string client_platform = null;
            public static string client_os = null;
            public static string client_kraken_session = null;
        }
    }

    public static class GameAuth
    {
        public static void ResolveUserID(string gameAuthResponse)
        {
            if (gameAuthResponse.IsJson() == true)
            {
                JObject json = JObject.Parse(gameAuthResponse);
                if (json.ContainsKey("userId") == true)
                {
                    Globals_Session.Game.userId = (string)json["userId"];
                }
            }
        }
    }
    public static class StringExtensions
    {
        // Метод расширения для проверки строки на JSON-формат
        public static bool IsJson(this string input)
        {
            try
            {
                JContainer.Parse(input); // Использует Newtonsoft.Json для проверки
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    internal class FiddlerCore
    {
        private static int FloadListTomes = 0;
        private static string dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BarasToolba");
        public static string wavFolderPath = Path.Combine(dataFolderPath, "completed.wav");

        public static string cfgQFolderPath = Path.Combine(dataFolderPath, "cfg.json");
        public static string TomeQFolderPath = Path.Combine(dataFolderPath, "tome.txt");
        public static string autoQFolderPath = Path.Combine(dataFolderPath, "auto.exe");

        static HttpClient WC = new HttpClient();
        private static string BaseDir = "https://raw.githubusercontent.com/S4CBS/ArchiveTool/branch2/internal/completed.wav";
        private static string autoBaseDir = "https://raw.githubusercontent.com/S4CBS/ArchiveTool/branch2/internal/auto.wav";
        public static string GetDataFolderPath()
        {
            try
            {
                if (!Directory.Exists(dataFolderPath))
                {
                    DwnloadSettings();
                    Directory.CreateDirectory(dataFolderPath);
                }

                if (!File.Exists(cfgQFolderPath))
                {
                    File.Create(cfgQFolderPath);
                }

                if (!File.Exists(TomeQFolderPath))
                {
                    File.Create(TomeQFolderPath);
                }

                return Directory.Exists(dataFolderPath) ? dataFolderPath : null;
            }
            catch
            {
                return null;
            }
        }

        async static Task DwnloadBytes(string url, string output)
        {
            try
            {
                byte[] fileBytes = await WC.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(output, fileBytes);
            }
            catch (Exception ex)
            {
            }
        }

        async static Task DwnloadSettings()
        {
            var downloadTasks = new List<Task>
            {
                DwnloadBytes(BaseDir, Path.Combine(dataFolderPath, "completed.wav")),
                DwnloadBytes(autoBaseDir, Path.Combine(dataFolderPath, "auto.exe"))
            };

            await Task.WhenAll(downloadTasks);
        }

        internal static class RootCertificate
        {
            public static string filePath = $"{GetDataFolderPath()}\\BarasToolba Root Certificate.p12";
            public static string passwordFilePath = $"{GetDataFolderPath()}\\BarasToolba Root Certificate Password.txt";

            public const string password = "QLa7X9G6mvNbHuhRjtAnSZ8f3y52DzpwCPeKFJBcVgxMq4dY";

            public static bool WritePasswordFile()
            {
                if (Directory.Exists(GetDataFolderPath()))
                {
                    try
                    {
                        File.WriteAllText(passwordFilePath, password);
                        return true;
                    }
                    catch
                    {
                        throw new Exception($"Failed to write certificate password file!");
                    }
                }

                return false;
            }




        }

        static FiddlerCore()
        {
            FiddlerApplication.BeforeRequest += FiddlerToCatchBeforeRequest;
            FiddlerApplication.AfterSessionComplete += FiddlerToCatchAfterSessionComplete;
        }

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        private static bool EnsureRootCertificate()
        {
            BCCertMaker.BCCertMaker certProvider = new BCCertMaker.BCCertMaker();
            CertMaker.oCertProvider = certProvider;

            if (File.Exists(RootCertificate.filePath) == false)
            {
                certProvider.CreateRootCertificate();
                certProvider.WriteRootCertificateAndPrivateKeyToPkcs12File(RootCertificate.filePath, RootCertificate.password);
            }
            else
            {
                try
                {
                    certProvider.ReadRootCertificateAndPrivateKeyFromPkcs12File(RootCertificate.filePath, RootCertificate.password);
                }
                catch
                {
                    File.Delete(RootCertificate.filePath); // Destroy corrupt certificate file.
                    EnsureRootCertificate(); // Re-execute function to build a new certificate.
                }
            }

            if (CertMaker.rootCertIsTrusted() == false)
            {
                return CertMaker.trustRootCert();
            }


            return true;

        }
        public static bool DestroyRootCertificates()
        {
            if (CertMaker.rootCertExists())
            {
                if (!CertMaker.removeFiddlerGeneratedCerts(true))
                    return false;
            }

            return true;
        }

        public static bool Start()
        {
            // AllocConsole();

            if (Form.PlatformBox.SelectedItem != null)
            {
                if (EnsureRootCertificate() == false)
                {
                    return false;
                }

                FiddlerApplication.Startup(new FiddlerCoreStartupSettingsBuilder().ListenOnPort(8888).RegisterAsSystemProxy().ChainToUpstreamGateway().DecryptSSL().OptimizeThreadPool().Build());
                Launcher.LaunchDBD(Form.PlatformBox.SelectedItem.ToString());
                Program.Host(Form.PlatformBox.SelectedItem.ToString());
                Form.PlatformBox.Enabled = false;
                return FiddlerApplication.IsStarted();
            }
            else
            {
                MessageBox.Show("Выбирите платформу", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return !FiddlerApplication.IsStarted();
        }

        public static void Stop()
        {
            if (FiddlerApplication.IsStarted())
            {

                if (Form.PlatformBox.SelectedItem != null)
                {
                    FiddlerApplication.Shutdown();
                    Launcher.KillDBD(Form.PlatformBox.SelectedItem.ToString());

                    Form.PriorityCheck.Enabled = true;
                    Form.ListTomes.Items.Clear();

                    Form.KQuest.Text = "Испытание: None";
                    Form.KProgress.Text = "Прогресс: None";
                    Form.KReward1.Text = "None";
                    Form.KReward2.Text = "None";

                    Form.Quest.Text = "Испытание: None";
                    Form.Progress.Text = "Прогресс: None";
                    Form.Reward1.Text = "None";
                    Form.Reward2.Text = "None";

                    Form.PlayerRole.Text = "Роль: xxx";
                    Form.Queue.Text = $"Очередь: xxx";
                    FloadListTomes = 0;
                    Form.PriorityCheck.Checked = false;
                    Globals_Session.Game.bhvrSession = null;
                    Form.PlatformBox.Enabled = true;
                    Globals_Session.Game.PLT = null;
                    Form.Prioritet = 0;
                }
            }
        }

        public static void FiddlerToCatchBeforeRequest(Session oSession)
        {
            if (oSession.uriContains("/login?token=") || oSession.uriContains("steam/loginWithTokenBody")){
                if (oSession.oRequest["User-Agent"].Length > 0)
                    Globals_Session.Game.user_agent = oSession.oRequest["User-Agent"];

                if (oSession.oRequest["x-kraken-client-platform"].Length > 0)
                    Globals_Session.Game.client_platform = oSession.oRequest["x-kraken-client-platform"];

                if (oSession.oRequest["x-kraken-client-provider"].Length > 0)
                    Globals_Session.Game.client_provider = oSession.oRequest["x-kraken-client-provider"];

                if (oSession.oRequest["x-kraken-client-os"].Length > 0)
                    Globals_Session.Game.client_os = oSession.oRequest["x-kraken-client-os"];

                if (oSession.oRequest["x-kraken-client-version"].Length > 0)
                    Globals_Session.Game.client_version = oSession.oRequest["x-kraken-client-version"];

                if (oSession.oRequest["x-kraken-analytics-session-id"].Length > 0)
                    Globals_Session.Game.client_kraken_session = oSession.oRequest["x-kraken-analytics-session-id"];
            }

            if (oSession.uriContains("/api/v1/config"))
            {
                if (oSession.oRequest["Cookie"].Length > 0)
                {
                    Globals_Session.Game.bhvrSession = oSession.oRequest["Cookie"].Replace("bhvrSession=", string.Empty);
                    UpdateData();
                    JsonHelper.SaveGameData();
                    Form.PriorityCheck.Enabled = false;
                }

                return;
            }
        }


        public static void FiddlerToCatchAfterSessionComplete(Session oSession)
        {
            if (oSession.uriContains("/login?token=") || oSession.uriContains("steam/loginWithTokenBody"))
            {
                oSession.utilDecodeResponse();
                GameAuth.ResolveUserID(oSession.GetResponseBodyAsString());
            }

            if (oSession.uriContains("/api/v1/archives/stories/update/active-node-v3"))
            {
                UpdateData();
            }

            if (oSession.uriContains("api/v1/archives/stories/get/story-status"))
            {
                if (FloadListTomes == 0)
                {
                    if (Form.Prioritet == 1)
                    {
                        oSession.utilDecodeResponse();
                        string resp = oSession.GetResponseBodyAsString();

                        if (string.IsNullOrEmpty(resp) == false)
                        {
                            if (resp.IsJson() == true)
                            {
                                JObject respJson = JObject.Parse(resp);

                                if (respJson.ContainsKey("eventStories") && respJson.ContainsKey("regularStories"))
                                {
                                    JArray eventStories = (JArray)respJson["eventStories"];
                                    JArray regularStories = (JArray)respJson["regularStories"];

                                    foreach (JObject tome in regularStories)
                                    {
                                        JArray levelStatus = (JArray)tome["levelStatus"];
                                        int count = 0;
                                        foreach (JObject status in levelStatus)
                                        {
                                            if (status["status"].ToString() == "mastered")
                                            {
                                                count = count + 1;
                                            }
                                        }

                                        if (count < 4)
                                        {
                                            Form.ListTomes.Items.Add(tome["id"]);
                                        }
                                        else
                                        {

                                        }
                                    }
                                    foreach (JObject tome in eventStories)
                                    {
                                        JArray levelStatus = (JArray)tome["levelStatus"];
                                        int count = 0;
                                        foreach (JObject status in levelStatus)
                                        {
                                            if (status["status"].ToString() == "mastered")
                                            {
                                                count = count + 1;
                                            }
                                        }
                                        if (count < 4)
                                        {
                                            Form.ListTomes.Items.Add(tome["id"]);
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                                else if (!respJson.ContainsKey("eventStories") && respJson.ContainsKey("regularStories"))
                                {
                                    JArray regularStories = (JArray)respJson["regularStories"];

                                    foreach (JObject tome in regularStories)
                                    {
                                        JArray levelStatus = (JArray)tome["levelStatus"];
                                        int count = 0;
                                        foreach (JObject status in levelStatus)
                                        {
                                            if (status["status"].ToString() == "mastered")
                                            {
                                                count = count + 1;
                                            }
                                        }

                                        if (count < 4)
                                        {
                                            Form.ListTomes.Items.Add(tome["id"]);
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (Form.Prioritet == 0)
                    {
                        Program.ClearFullFile(TomeQFolderPath);
                        oSession.utilDecodeResponse();
                        string resp = oSession.GetResponseBodyAsString();

                        if (string.IsNullOrEmpty(resp) == false)
                        {
                            if (resp.IsJson() == true)
                            {
                                JObject respJson = JObject.Parse(resp);

                                if (respJson.ContainsKey("eventStories") && respJson.ContainsKey("regularStories"))
                                {
                                    JArray eventStories = (JArray)respJson["eventStories"];
                                    JArray regularStories = (JArray)respJson["regularStories"];

                                    foreach (JObject tome in regularStories)
                                    {
                                        JArray levelStatus = (JArray)tome["levelStatus"];
                                        int count = 0;
                                        foreach (JObject status in levelStatus)
                                        {
                                            if (status["status"].ToString() == "mastered")
                                            {
                                                count = count + 1;
                                            }
                                        }

                                        if (count < 4)
                                        {
                                            Form.ListTomes.Items.Add(tome["id"]);
                                            Program.Zapis(TomeQFolderPath, tome["id"].ToString());
                                        }
                                        else
                                        {

                                        }
                                    }
                                    foreach (JObject tome in eventStories)
                                    {
                                        JArray levelStatus = (JArray)tome["levelStatus"];
                                        int count = 0;
                                        foreach (JObject status in levelStatus)
                                        {
                                            if (status["status"].ToString() == "mastered")
                                            {
                                                count = count + 1;
                                            }
                                            if (status.ContainsKey("hasUnseenContent"))
                                            {
                                                if (status["hasUnseenContent"].ToString() == "True")
                                                {
                                                }
                                                else
                                                {
                                                    if (count < 4)
                                                    {
                                                        Form.ListTomes.Items.Add(tome["id"]);
                                                        Program.Zapis(TomeQFolderPath, tome["id"].ToString());
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (!respJson.ContainsKey("eventStories") && respJson.ContainsKey("regularStories"))
                                {
                                    JArray regularStories = (JArray)respJson["regularStories"];

                                    foreach (JObject tome in regularStories)
                                    {
                                        JArray levelStatus = (JArray)tome["levelStatus"];
                                        int count = 0;
                                        foreach (JObject status in levelStatus)
                                        {
                                            if (status["status"].ToString() == "mastered")
                                            {
                                                count = count + 1;
                                            }
                                        }

                                        if (count < 4)
                                        {
                                            Form.ListTomes.Items.Add(tome["id"]);
                                            Program.Zapis(TomeQFolderPath, tome["id"].ToString());
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                FloadListTomes = 1;
            }

            if (oSession.uriContains("/api/v1/queue"))
            {
                if (oSession.fullUrl.EndsWith("/token/issue"))
                    return;

                if (oSession.fullUrl.EndsWith("/cancel"))
                {
                    Form.Queue.Text = $"Очередь: xxx";
                }

                Globals_Session.Game.isInMatch = false;

                Globals_Session.Game.matchId = null;
                Globals_Session.Game.matchType = Globals_Session.Game.E_MatchType.None;
                Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.None;

                oSession.utilDecodeResponse();
                string responseBody = oSession.GetResponseBodyAsString();

                if (string.IsNullOrEmpty(responseBody) == false)
                {
                    if (responseBody.IsJson() == true)
                    {
                        JObject queueJson = JObject.Parse(responseBody);
                        if ((string)queueJson["status"] == "QUEUED")
                        {
                            Form.Queue.Text = $"Очередь: {queueJson["queueData"]["position"].ToString()}";
                        }
                        else if ((string)queueJson["status"] == "MATCHED")
                            Form.Queue.Text = $"Очередь: xxx";
                        else
                            Form.Queue.Text = $"Очередь: xxx";
                    }
                }
                return;
            }

            if (oSession.uriContains("/api/v1/match"))
            {
                oSession.utilDecodeResponse();
                string responseBody = oSession.GetResponseBodyAsString();

                if (responseBody.IsJson())
                {
                    JObject responseJson = JObject.Parse(responseBody);

                    if (responseJson.ContainsKey("sideA") && responseJson.ContainsKey("sideB"))
                    {
                        JArray survivorsArray = (JArray)responseJson["sideB"];
                        JArray killersArray = (JArray)responseJson["sideA"];

                        foreach (string killer in killersArray)
                        {
                            if (killer == Globals_Session.Game.userId)
                            {
                                Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.Killer;
                                Form.PlayerRole.Text =  $"Роль: {Globals_Session.Game.playerRole.ToString()}";
                            }
                        }

                        foreach (string survivor in survivorsArray)
                        {
                            if (survivor == Globals_Session.Game.userId)
                            {
                                Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.Survivor;
                                Form.PlayerRole.Text = $"Роль: {Globals_Session.Game.playerRole.ToString()}";
                            }
                        }

                    }
                    if (responseJson.ContainsKey("status") && responseJson.ContainsKey("reason"))
                    {
                        if ((string)responseJson["status"] == "CLOSED" && (string)responseJson["reason"] == "closed")
                        {
                            if (Globals_Session.Game.isInMatch == false)
                            {
                                if (responseJson.ContainsKey("props"))
                                {
                                    if ((string)responseJson["props"]["GameType"] == ":1") // GameType :1 - Custom Game. We do not want our Queue Status logic to apply to the custom match.
                                    {
                                        // Globals_Session.Game.matchType = Globals_Session.Game.E_MatchType.Custom;
                                        Globals_Session.Game.matchType = Globals_Session.Game.E_MatchType.Default;
                                    }
                                    else
                                    {
                                        Globals_Session.Game.matchType = Globals_Session.Game.E_MatchType.Default;
                                    }
                                }

                                Globals_Session.Game.isInMatch = true; // Match was found and closed, at this point player is already loading in.
                            }
                        }
                    }

                    string searchPattern = @"match\/([a-f0-9\-]+)$"; // "/api/v1/match/{GUID}"
                    Match match = Regex.Match(oSession.fullUrl, searchPattern);
                    if (match.Success)
                    {
                        Globals_Session.Game.matchId = match.Groups[1].Value;
                    }

                }
            }

            if (oSession.uriContains("/api/v1/gameDataAnalytics/v2/batch"))
            {
                oSession.utilDecodeRequest();
                string requestBody = oSession.GetRequestBodyAsString();

                if (requestBody.IsJson())
                {
                    JObject requestJson = JObject.Parse(requestBody);
                    {
                        if (requestJson.ContainsKey("events"))
                        {
                            foreach (JToken gameEvent in requestJson["events"])
                            {
                                JToken eventData = gameEvent["data"];

                                if (eventData != null)
                                {
                                    JToken matchId = eventData["match_id"];
                                    JToken krakenMatchId = eventData["kraken_match_id"];

                                    if (matchId != null && krakenMatchId != null)
                                    {
                                        Archives.S_MatchData matchData = new Archives.S_MatchData((string)matchId, (string)krakenMatchId);
                                        Archives.CompleteActiveQuest(matchData);
                                    }
                                    else
                                    {
                                        matchId = eventData["matchmaking_session_guid"];

                                        if (matchId != null && krakenMatchId != null)
                                        {
                                            Archives.S_MatchData matchData = new Archives.S_MatchData((string)matchId, (string)krakenMatchId);
                                            Archives.CompleteActiveQuest(matchData);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void UpdateData()
        {
            // Проверка наличия активной сессии
            if (Globals_Session.Game.bhvrSession != null)
            {
                try
                {
                    // Настройка HTTP-клиента с отключенной проверкой SSL 
                    using (var handler = new HttpClientHandler())
                    {
                        // Опасная настройка! Отключает проверку SSL-сертификатов
                        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                        using (var client = new HttpClient(handler))
                        {
                            // Добавление заголовков для аутентификации и идентификации
                            client.DefaultRequestHeaders.Add("Cookie", $"bhvrSession={Globals_Session.Game.bhvrSession}");
                            client.DefaultRequestHeaders.Add("x-kraken-analytics-session-id", Globals_Session.Game.client_kraken_session);
                            client.DefaultRequestHeaders.Add("x-kraken-client-platform", Globals_Session.Game.client_platform);
                            client.DefaultRequestHeaders.Add("x-kraken-client-provider", Globals_Session.Game.client_provider);
                            client.DefaultRequestHeaders.Add("x-kraken-client-os", Globals_Session.Game.client_os);
                            client.DefaultRequestHeaders.UserAgent.ParseAdd(Globals_Session.Game.user_agent);

                            // Запрос данных об активных квестах
                            string urlGetStory = $"https://{Globals_Session.Game.PLT}/api/v1/archives/stories/get/activeNode";
                            var response = client.GetAsync(urlGetStory).Result; // Блокирующий вызов!
                            response.EnsureSuccessStatusCode(); // Проверка HTTP 200 OK
                            var responseBody = response.Content.ReadAsStringAsync().Result; // Синхронное чтение

                            // Парсинг JSON-ответа
                            var json = JObject.Parse(responseBody);
                            var activeNode = json["activeNode"] as JArray; // Список всех доступных нод

                            // Неиспользуемые переменные (возможно, остатки кода)
                            string survNode = string.Empty;
                            string killNode = string.Empty;

                            // Обработка данных для выжившего
                            try
                            {
                                // Получение активной ноды выжившего
                                var survivorActiveNode = json["survivorActiveNode"]["nodeId"];

                                // Поиск соответствующей ноды в общем списке
                                foreach (var data in activeNode)
                                {
                                    // Сравнение идентификаторов нод
                                    if (data["nodeTreeCoordinate"]["nodeId"].ToString() == survivorActiveNode.ToString())
                                    {
                                        // Обновление UI
                                        Form.Quest.Text = "Испытание: " + data["clientInfoId"].ToString();
                                        Form.Progress.Text = "Прогресс: " + $"{data["objectives"][0]["currentProgress"]}/{data["objectives"][0]["neededProgression"]}";

                                        // Обработка наград
                                        if (data["rewards"] != null && data["rewards"].HasValues)
                                        {
                                            var rewards = data["rewards"].Take(2).ToList();

                                            // Первая награда
                                            Form.Reward1.Text = rewards.Count > 0
                                                ? $"{rewards[0]["id"]} = {rewards[0]["amount"]}"
                                                : "None";

                                            // Вторая награда
                                            Form.Reward2.Text = rewards.Count > 1
                                                ? $"{rewards[1]["id"]} = {rewards[1]["amount"]}"
                                                : "None";
                                        }
                                        else
                                        {
                                            // Сброс наград при их отсутствии
                                            Form.Reward1.Text = "None";
                                            Form.Reward2.Text = "None";
                                        }
                                    }
                                }
                                // Сохранение данных сессии
                                Globals_Session.Game.survivor = new JObject
                                {
                                    ["level"] = json["survivorActiveNode"]["level"],
                                    ["nodeId"] = json["survivorActiveNode"]["nodeId"],
                                    ["storyId"] = json["survivorActiveNode"]["storyId"]
                                };
                            }
                            catch (Exception ex)
                            {
                                // Сброс UI при ошибках
                                Form.Quest.Text = "Испытание: None";
                                Form.Progress.Text = "Прогресс: None";
                                Form.Reward1.Text = "None";
                                Form.Reward2.Text = "None";

                                // Очистка данных сессии
                                Globals_Session.Game.survivor = null;
                            }

                            // Обработка данных для убийцы (аналогично выжившему)
                            try
                            {
                                var killerActiveNode = json["killerActiveNode"]["nodeId"];

                                foreach (var data in activeNode)
                                {
                                    if (data["nodeTreeCoordinate"]["nodeId"].ToString() == killerActiveNode.ToString())
                                    {
                                        // Обновление UI для убийцы
                                        Form.KQuest.Text = "Испытание: " + data["clientInfoId"].ToString();
                                        Form.KProgress.Text = "Прогресс: " + $"{data["objectives"][0]["currentProgress"]}/{data["objectives"][0]["neededProgression"]}";

                                        // Обработка наград убийцы
                                        if (data["rewards"] != null && data["rewards"].HasValues)
                                        {
                                            var Krewards = data["rewards"].Take(2).ToList();

                                            Form.KReward1.Text = Krewards.Count > 0
                                                ? $"{Krewards[0]["id"]} = {Krewards[0]["amount"]}"
                                                : "None";

                                            Form.KReward2.Text = Krewards.Count > 1
                                                ? $"{Krewards[1]["id"]} = {Krewards[1]["amount"]}"
                                                : "None";
                                        }
                                        else
                                        {
                                            Form.KReward1.Text = "None";
                                            Form.KReward2.Text = "None";
                                        }
                                    }
                                }
                                Globals_Session.Game.killer = json["killerActiveNode"];
                            }
                            catch (Exception ex)
                            {
                                // Сброс UI для убийцы
                                Form.KQuest.Text = "Испытание: None";
                                Form.KProgress.Text = "Прогресс: None";
                                Form.KReward1.Text = "None";
                                Form.KReward2.Text = "None";

                                // Сохранение данных сессии
                                Globals_Session.Game.killer = new JObject
                                {
                                    ["level"] = json["killerActiveNode"]["level"],
                                    ["nodeId"] = json["killerActiveNode"]["nodeId"],
                                    ["storyId"] = json["killerActiveNode"]["storyId"]
                                };
                            }
                        }
                    }
                }
                catch // Глобальный перехватчик исключений (не рекомендуется)
                {
                    // Пустой блок catch - антипаттерн! Ошибки игнорируются
                }
            }
        }
    }
}
