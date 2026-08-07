using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogAnalyzer.Data
{
    internal class Initializer
    {
        public static class StartupInitializer
        {
            /// <summary>
            /// Проверяет существование всех необходимых папок и файлов.
            /// Если чего-то нет – создаёт с корректным содержимым по умолчанию.
            /// </summary>
            public static async Task EnsureAppInfrastructureAsync()
            {
                var settings = LogAnalyzerSettings.Setting;

                // 1. Корневая папка (Documents/MyAnalyzerLog)
                string root = settings.DefaultSavePath;
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                    await Get_log.LoggerAsync($"Создана корневая папка: {root}");
                }

                // 2. Папка для настроек (Documents/MyAnalyzerLog/SettingJson)
                string settingsFolder = settings.SettingsFolderPath;
                if (!Directory.Exists(settingsFolder))
                {
                    Directory.CreateDirectory(settingsFolder);
                    await Get_log.LoggerAsync($"Создана папка настроек: {settingsFolder}");
                }

                // 3. Файл setting.json
                string settingsFile = settings.SettingsFilePath;
                if (!File.Exists(settingsFile))
                {
                    await SaveDefaultSettingsAsync(settingsFile, settings);
                    await Get_log.LoggerAsync($"Создан файл настроек: {settingsFile}");
                }

                // 4. Файл notes.json (если нужен – иначе можно пропустить)
                string notesFile = settings.NotesFilePath;
                if (!File.Exists(notesFile))
                {
                    // Создаём пустой массив JSON, чтобы при чтении не было ошибки
                    await File.WriteAllTextAsync(notesFile, "[]");
                    await Get_log.LoggerAsync($"Создан файл заметок: {notesFile}");
                }
            }

            private static async Task SaveDefaultSettingsAsync(string filePath, LogAnalyzerSettings settings)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(settings, options);
                await File.WriteAllTextAsync(filePath, json);
            }
            
        }
    }
}
