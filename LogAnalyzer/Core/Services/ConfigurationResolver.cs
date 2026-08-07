using LogAnalyzer.Data;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using MyNotes.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.Services
{
    public static class ConfigurationResolver
    {
        /// <summary>
        /// Загружает активную конфигурацию на основе аргументов.
        /// Приоритет: -c > локальный setting.json рядом с логами > глобальный.
        /// </summary>
        public static async Task<LogAnalyzerSettings> ResolveAsync(CommandLineOptions options)
        {
            LogAnalyzerSettings active = LogAnalyzerSettings.Setting; // уже загруженный по умолчанию

            if (!string.IsNullOrEmpty(options.ConfigFilePath))
            {
                var loaded = await LoadAndSave.LoadAndSaveAsync(options.ConfigFilePath);
                if (loaded != null)
                {
                    active = loaded;
                    LogAnalyzerSettings.Setting = loaded;
                    await Get_log.LoggerAsync($"[INFO] [ConfigurationResolver] Конфиг загружен из -c | path='{options.ConfigFilePath}'");
                }
                else
                {
                    await Get_log.LoggerAsync("[WARN] [ConfigurationResolver] Не удалось загрузить конфиг из -c, используются настройки по умолчанию");
                }
            }
            else if (!string.IsNullOrEmpty(options.InputFilePath))
            {
                // Ищем setting.json в папке логов
                string sourcePath = options.InputFilePath;
                if (File.Exists(sourcePath))
                    sourcePath = Path.GetDirectoryName(sourcePath);

                string localConfig = Path.Combine(sourcePath, "setting.json");
                if (File.Exists(localConfig))
                {
                    var loaded = await LoadAndSave.LoadAndSaveAsync(localConfig);
                    if (loaded != null)
                    {
                        active = loaded;
                        LogAnalyzerSettings.Setting = loaded;
                        await Get_log.LoggerAsync($"[INFO] [ConfigurationResolver] Конфиг найден в папке логов | path='{localConfig}'");
                    }
                }
                else
                {
                    await Get_log.LoggerAsync($"[INFO] [ConfigurationResolver] Конфиг в папке логов не найден, используются глобальные настройки | searchedPath='{localConfig}'");
                }
            }

            return active;
        }
    }
}