using System.Media;
using BarasToolba;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System.Diagnostics;

namespace BarasToolba
{
    public static class Media
    {
        static System.Media.SoundPlayer player = new System.Media.SoundPlayer(FiddlerCore.wavFolderPath);
        public static void playSound()
        {
            player.Play();
        }
    }

    public static class JsonHelper
    {
        public static void SaveGameData()
        {
            var gameData = new
            {
                bhvrSession = Globals_Session.Game.bhvrSession,
                x_kraken_analytics_session_id = Globals_Session.Game.client_kraken_session,
                useragent = Globals_Session.Game.user_agent,
                platform = Globals_Session.Game.client_platform,
                provider = Globals_Session.Game.client_provider,
                os = Globals_Session.Game.client_os,
                version = Globals_Session.Game.client_version
            };
            string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            File.WriteAllText(FiddlerCore.cfgQFolderPath, json);
        }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form());
        }

        internal static void RoleSpoffer()
        {
            // получаем текущюю роль.
            var role = Globals_Session.Game.playerRole.ToString();

            if (role != "None")
            {
                if (role == "Survivor")
                {
                    Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.Killer;
                    Form.PlayerRole.Text = $"Роль: {Globals_Session.Game.playerRole.ToString()}";
                }
                else if (role == "Killer")
                {
                    Globals_Session.Game.playerRole = Globals_Session.Game.E_PlayerRole.Survivor;
                    Form.PlayerRole.Text = $"Роль: {Globals_Session.Game.playerRole.ToString()}";
                }
            }
            else
            {
                MessageBox.Show("Вы не в матче", "Подмена не удалась.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static async Task PickNewQuestAsync()
        {
            try
            {
                // Проверка существования файла
                if (!File.Exists(FiddlerCore.autoQFolderPath))
                {
                    Console.WriteLine($"Файл {FiddlerCore.autoQFolderPath} не найден.");
                    return;
                }

                // Запуск первого процесса
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = FiddlerCore.autoQFolderPath;
                    process.StartInfo.CreateNoWindow = false;
                    process.Start();
                    await Task.Run(() => process.WaitForExit()); // Асинхронное ожидание завершения
                    
                }

                // Запуск второго процесса
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = FiddlerCore.autoQFolderPath;
                    process.StartInfo.CreateNoWindow = false;
                    process.Start();
                    await Task.Run(() => process.WaitForExit()); // Асинхронное ожидание завершения
                    
                }

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = FiddlerCore.autoQFolderPath;
                    process.StartInfo.CreateNoWindow = false;
                    process.Start();
                    await Task.Run(() => process.WaitForExit()); // Асинхронное ожидание завершения
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске auto.exe: {ex.Message}");
            }
        }

        public static void ClearFullFile(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.Truncate))
            {
                // Просто открываем файл в режиме Truncate — содержимое удаляется
            }
        }

        public static void Zapis(string path, string text)
        {
            using (var tw = new StreamWriter(path, true))
            {
                tw.WriteLine(text);
            }
        }

        public static void Host(string plt)
        {
            if (plt == "EGS")
            {
                Globals_Session.Game.PLT = "egs.live.bhvrdbd.com";
            }
            else if (plt == "Steam")
            {
                Globals_Session.Game.PLT = "steam.live.bhvrdbd.com";
            }
            else if (plt == "MS")
            {
                Globals_Session.Game.PLT = "grdk.live.bhvrdbd.com";
            }
        }
    }
}