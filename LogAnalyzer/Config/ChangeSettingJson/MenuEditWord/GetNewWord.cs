using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;

namespace LogAnalyzer.Config.ChangeSettingJson.MenuEditWord
{
    internal class GetNewWord
    {
        public static List<string> ListPathInLog = ChangingFilePath.ListPathInLog;

        public static HashSet<string> MyWordList { get; set; }


        // Перегрузка 1: Стандартный вызов без параметров командной строки
        public static void CallGetNewWord(LogAnalyzerSettings LinkSetting)
        {
            // Если путь пустой, берем дефолтный
            string configPath = ListPathInLog?.FirstOrDefault() ?? LinkSetting.SettingsFilePath;
            
            // Загружаем актуальные слова из файла перед стартом
            LoadWordsFromFile(configPath, LinkSetting);

            ExecuteWordLoop(LinkSetting, configPath);
        }

        // Перегрузка 2: Вызов с аргументами командной строки
        public static void CallGetNewWord(LogAnalyzerSettings LinkSetting, string[] args)
        {
            string customPath = null;

            // Парсим аргументы
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-cfg" || args[i] == "-sjp" || args[i] == "--config")
                {
                    customPath = args[i + 1];
                    break;
                }
            }

            // Определяем приоритетный путь для чтения/записи
            string configPath = customPath ?? ListPathInLog?.FirstOrDefault() ?? LinkSetting.SettingsFilePath;

            if (!string.IsNullOrEmpty(customPath))
            {
                LinkSetting.SettingsFilePath = customPath;
                Console.WriteLine($"[Config] Путь изменен через CLI: {customPath}");
            }

            // Принудительно читаем слова из выбранного файла
            LoadWordsFromFile(configPath, LinkSetting);

            ExecuteWordLoop(LinkSetting, configPath);
        }

        // Метод для чтения данных из файла конфигурации
        private static void LoadWordsFromFile(string path, LogAnalyzerSettings linkSetting)
        {
            var MyWordListLink = linkSetting.MyWordList;
            // Защита: если список слов вообще не создан, создаем его вручную
            if (MyWordListLink == null)
            {
                MyWordListLink = new HashSet<string>();
            }

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    var loadedSettings = LoadAndSave.LoadAndSaveAsync(path).GetAwaiter().GetResult();

                    // Если десериализатор вернул null, выходим без падения
                    if (loadedSettings == null) return;

                    Task.Delay(1000).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Warning] Не удалось прочитать слова из файла: {ex.Message}");
                    Task.Delay(2000).GetAwaiter().GetResult();
                }
            }
        }

        // Основной цикл работы
        private static void ExecuteWordLoop(LogAnalyzerSettings LinkSetting, string targetSavePath)
        {
            var MyWordListLink = LinkSetting.MyWordList;

            bool work = true;

            while (work)
            {
                Console.Clear();
                Console.WriteLine("=== Текущие слова в списке ===");
                foreach (var item in LinkSetting.MyWordList)
                {
                    Console.WriteLine($"- {item}");
                }
                Console.WriteLine("==============================\n");

                Console.WriteLine("To exit, input in console 'stop - s' or 'exit - e'");
                string messageUser = Check.ReadLine("input search to word to check");

                if (messageUser == "exit" || messageUser == "stop" || messageUser == "e" || messageUser == "s")
                {
                    work = false;
                    Console.Clear();
                    continue;
                }

                // Добавляем в HashSet (дубликаты отсекутся автоматически)
                MyWordListLink.Add(messageUser);
                Console.Clear();

                Console.WriteLine("Do you want to save changes to JSON? (y/n)");
                string saveChoice = Console.ReadLine()?.ToLower();

                if (saveChoice == "y" || saveChoice == "yes")
                {
                    if (!string.IsNullOrEmpty(targetSavePath))
                    {
                        string directory = Path.GetDirectoryName(targetSavePath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Сохраняем актуальный объект в JSON
                        LoadAndSave.SaveToJsonAsync(targetSavePath, LinkSetting).GetAwaiter().GetResult();
                        Console.WriteLine($"Saved successfully to: {targetSavePath}");
                        Task.Delay(1500).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Console.WriteLine("Error: Could not determine any save path.");
                        Task.Delay(2000).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}
