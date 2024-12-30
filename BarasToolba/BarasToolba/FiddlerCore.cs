using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using BarasToolba;
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




            public static string bhvrSession = null;
            public static string userId = null;


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
        private static string dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BarasToolba");
        public static string wavFolderPath = Path.Combine(dataFolderPath, "completed.wav");
        static HttpClient WC = new HttpClient();
        private static string BaseDir = "https://raw.githubusercontent.com/S4CBS/ArchiveTool/main/internal/completed.wav";
        public static string GetDataFolderPath()
        {
            try
            {
                if (!Directory.Exists(dataFolderPath))
                {
                    DwnloadSettings();
                    Directory.CreateDirectory(dataFolderPath);
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
                DwnloadBytes(BaseDir, Path.Combine(dataFolderPath, "completed.wav"))
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

            if (EnsureRootCertificate() == false)
            {
                return false;
            }

            FiddlerApplication.Startup(new FiddlerCoreStartupSettingsBuilder().ListenOnPort(8888).RegisterAsSystemProxy().ChainToUpstreamGateway().DecryptSSL().OptimizeThreadPool().Build());
            Launcher.LaunchDBD("EGS");
            return FiddlerApplication.IsStarted();
        }

        public static void Stop()
        {
            if (FiddlerApplication.IsStarted())
            {
                FiddlerApplication.Shutdown();
                Launcher.KillDBD("EGS");
            }
        }

        public static void FiddlerToCatchBeforeRequest(Session oSession)
        {
            if (oSession.uriContains("")){
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
                }

                return;
            }
        }


        public static void FiddlerToCatchAfterSessionComplete(Session oSession)
        {
            if (oSession.uriContains("/login?token="))
            {
                oSession.utilDecodeResponse();
                GameAuth.ResolveUserID(oSession.GetResponseBodyAsString());
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
                                        Globals_Session.Game.matchType = Globals_Session.Game.E_MatchType.Custom;
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
                                }
                            }
                        }
                    }
                }
            }
        }


        public static void UpdateData()
        {
            if (Globals_Session.Game.bhvrSession != null)
            {
                try
                {
                    using (var handler = new HttpClientHandler())
                    {
                        // Отключаем проверку SSL (не рекомендуется для продакшн)
                        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                        using (var client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Cookie", $"bhvrSession={Globals_Session.Game.bhvrSession}");
                            client.DefaultRequestHeaders.Add("x-kraken-analytics-session-id", Globals_Session.Game.client_kraken_session);
                            client.DefaultRequestHeaders.Add("x-kraken-client-platform", Globals_Session.Game.client_platform);
                            client.DefaultRequestHeaders.Add("x-kraken-client-provider", Globals_Session.Game.client_provider);
                            client.DefaultRequestHeaders.Add("x-kraken-client-os", Globals_Session.Game.client_os);
                            client.DefaultRequestHeaders.UserAgent.ParseAdd(Globals_Session.Game.user_agent);

                            // 1. Получение Story ID
                            string urlGetStory = "https://egs.live.bhvrdbd.com/api/v1/archives/stories/get/story?storyId=Tome01";
                            var response = client.GetAsync(urlGetStory).Result;
                            response.EnsureSuccessStatusCode();
                            var responseBody = response.Content.ReadAsStringAsync().Result;

                            var json = JObject.Parse(responseBody);
                            var activeNodes = json["activeNodes"] as JArray;

                            if (activeNodes != null && activeNodes.Count > 0)
                            {
                                var data = activeNodes[0];

                                // Подготавливаем данные для следующего запроса
                                var payload = new
                                {
                                    node = data
                                };
                                string payloadJson = JsonConvert.SerializeObject(payload);

                                // 2. Обновление active-node-v3 (первый запрос)
                                string urlUpdateNode = "https://egs.live.bhvrdbd.com/api/v1/archives/stories/update/active-node-v3";
                                var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                                response = client.PostAsync(urlUpdateNode, content).Result;
                                response.EnsureSuccessStatusCode();
                                responseBody = response.Content.ReadAsStringAsync().Result;

                                // 3. Повторный запрос с дополнительным заголовком
                                client.DefaultRequestHeaders.Add("Block", "true");
                                content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                                response = client.PostAsync(urlUpdateNode, content).Result;
                                response.EnsureSuccessStatusCode();
                                responseBody = response.Content.ReadAsStringAsync().Result;

                                // 4. Обработка ответа
                                json = JObject.Parse(responseBody);
                                var activeNodesFull = json["activeNodesFull"] as JArray;

                                if (activeNodesFull != null && activeNodesFull.Count > 0)
                                {
                                    var activeNode = json["activeNodesFull"][0];
                                    string storyId = data["storyId"].ToString();
                                    string clientInfoId = activeNode["clientInfoId"].ToString();
                                    int currentProgress = (int)activeNode["objectives"][0]["currentProgress"];
                                    int neededProgression = (int)activeNode["objectives"][0]["neededProgression"];
                                    var questEventId = activeNode["objectives"][0]["questEvent"][0];

                                    var progress = neededProgression;

                                    if (activeNode["rewards"] != null && activeNode["rewards"].HasValues)
                                    {
                                        var rewards = activeNode["rewards"].Take(2).ToList();

                                        if (rewards.Count > 0)
                                        {
                                            // Обновляем Reward1 первой наградой
                                            Form.Reward1.Text = $"{rewards[0]["id"]} = {rewards[0]["amount"]}";
                                        }
                                        else
                                        {
                                            // Если наград нет, очищаем Reward1
                                            Form.Reward1.Text = "Нет награды";
                                        }

                                        if (rewards.Count > 1)
                                        {
                                            // Обновляем Reward2 второй наградой
                                            Form.Reward2.Text = $"{rewards[1]["id"]} = {rewards[1]["amount"]}";
                                        }
                                        else
                                        {
                                            // Если только одна награда, очищаем Reward2
                                            Form.Reward2.Text = "Нет награды";
                                        }
                                    }
                                    else
                                    {
                                        Form.Reward1.Text = "Нет награды";
                                        Form.Reward2.Text = "Нет награды";
                                    }

                                    Form.Quest.Text = $"Текущее испытание: {clientInfoId} ({storyId})";
                                    Form.Progress.Text = $"Прогресс: {currentProgress}/{neededProgression}";
                                }
                                else
                                {
                                    Form.Quest.Text = "Текущее испытание:";
                                    Form.Progress.Text = "Прогресс:";
                                    Form.Reward1.Text = "Нет награды";
                                    Form.Reward2.Text = "Нет награды";
                                }
                            }
                            else
                            {
                                Form.Quest.Text = "Текущее испытание:";
                                Form.Progress.Text = "Прогресс:";
                                Form.Reward1.Text = "Нет награды";
                                Form.Reward2.Text = "Нет награды";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Cookie не получены!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
