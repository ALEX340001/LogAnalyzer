// ================================================================
//  LoadAndSave.cs
// ================================================================
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave
{
    public class LoadAndSave
    {
        public static LogAnalyzerSettings settingsToSave = LogAnalyzerSettings.Setting;

        public static async Task<LogAnalyzerSettings> LoadAndSaveAsync(string fileName)
        {
            await Get_log.LoggerAsync($"[DEBUG] [LoadAndSave.LoadAndSaveAsync] Вход в метод | fileName='{fileName}'");
            try
            {
                if (!File.Exists(fileName))
                {
                    await Get_log.LoggerAsync("[INFO] [LoadAndSave.LoadAndSaveAsync] Файл настроек не найден, созданы настройки по умолчанию");
                    return new LogAnalyzerSettings();
                }

                string json = await File.ReadAllTextAsync(fileName);
                var settings = JsonSerializer.Deserialize<LogAnalyzerSettings>(json) ?? new LogAnalyzerSettings();
                await Get_log.LoggerAsync("[INFO] [LoadAndSave.LoadAndSaveAsync] Настройки загружены успешно");
                await Get_log.LoggerAsync("[DEBUG] [LoadAndSave.LoadAndSaveAsync] Выход из метода | result=LogAnalyzerSettings");
                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке настроек: {ex.Message}");
                await Get_log.LoggerAsync($"[ERROR] [LoadAndSave.LoadAndSaveAsync] Ошибка загрузки настроек | exception='{ex.GetType().Name}', message='{ex.Message}', fileName='{fileName}'");
                return new LogAnalyzerSettings();
            }
        }

        public static async Task SaveToJsonAsync(string fileName, LogAnalyzerSettings setting)
        {
            await Get_log.LoggerAsync($"[DEBUG] [LoadAndSave.SaveToJsonAsync] Вход в метод | fileName='{fileName}'");
            try
            {
                string json = JsonSerializer.Serialize(setting);
                await File.WriteAllTextAsync(fileName, json);
                await Get_log.LoggerAsync($"[INFO] [LoadAndSave.SaveToJsonAsync] Настройки сохранены в JSON | length={json.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error_SaveToJson: {ex.Message}");
                await Get_log.LoggerAsync($"[ERROR] [LoadAndSave.SaveToJsonAsync] Ошибка сохранения JSON | exception='{ex.GetType().Name}', message='{ex.Message}', fileName='{fileName}'");
            }
            await Get_log.LoggerAsync("[DEBUG] [LoadAndSave.SaveToJsonAsync] Выход из метода | result=успешно");
        }

        public static async Task SaveSettingsProcess()
        {
            await Get_log.LoggerAsync("[DEBUG] [LoadAndSave.SaveSettingsProcess] Вход в метод | параметры: отсутствуют");
            Console.Clear();
            string defaultPath = LogAnalyzerSettings.Setting.SettingsFolderPath;
            string defaultFile = LogAnalyzerSettings.Setting.SettingsFilePath;

            try
            {
                Console.WriteLine($"Нажмите Enter, чтобы сохранить в папку по умолчанию:");
                Console.WriteLine($"{defaultPath}");
                Console.Write("Или введите новый путь к папке: ");
                string inputPath = Console.ReadLine();
                string finalFilePath;

                if (string.IsNullOrWhiteSpace(inputPath))
                {
                    finalFilePath = defaultFile;
                    await CheckingDefaultFolder(defaultPath);
                }
                else
                {
                    if (!Directory.Exists(inputPath)) Directory.CreateDirectory(inputPath);
                    finalFilePath = Path.Combine(inputPath, "setting.json");
                }

                string json = JsonSerializer.Serialize(settingsToSave, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(finalFilePath, json);
                Console.WriteLine($"Настройки успешно сохранены: {finalFilePath}");
                await Get_log.LoggerAsync($"[INFO] [LoadAndSave.SaveSettingsProcess] Настройки сохранены | path='{finalFilePath}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                await Get_log.LoggerAsync($"[ERROR] [LoadAndSave.SaveSettingsProcess] Ошибка сохранения | exception='{ex.GetType().Name}', message='{ex.Message}'");
            }
            await Get_log.LoggerAsync("[DEBUG] [LoadAndSave.SaveSettingsProcess] Выход из метода | result=успешно");
        }

        public static async Task CheckingDefaultFolder(string folderPath)
        {
            await Get_log.LoggerAsync($"[DEBUG] [LoadAndSave.CheckingDefaultFolder] Вход в метод | folderPath='{folderPath}'");
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Console.WriteLine($"Папка создана: {folderPath}");
                    await Get_log.LoggerAsync($"[INFO] [LoadAndSave.CheckingDefaultFolder] Папка создана | path='{folderPath}'");
                }

                string settingsFile = LogAnalyzerSettings.Setting.SettingsFilePath;
                if (!File.Exists(settingsFile))
                {
                    await Get_log.LoggerAsync("[INFO] [LoadAndSave.CheckingDefaultFolder] Файл настроек не найден, создаётся новый");
                    await LoadAndSaveAsync(settingsFile);
                    Console.WriteLine($"Файл настроек создан: {settingsFile}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в CheckingDefaultFolder: {ex.Message}");
                await Get_log.LoggerAsync($"[ERROR] [LoadAndSave.CheckingDefaultFolder] Ошибка проверки папки | exception='{ex.GetType().Name}', message='{ex.Message}', folderPath='{folderPath}'");
            }
            await Get_log.LoggerAsync("[DEBUG] [LoadAndSave.CheckingDefaultFolder] Выход из метода | result=успешно");
        }

        public static async Task InputPathNotesJson(string path)
        {
            await Get_log.LoggerAsync($"[DEBUG] [LoadAndSave.InputPathNotesJson] Вход в метод | path='{path}'");
            try
            {
                string finalPath = path;
                if (Directory.Exists(path) || !path.EndsWith(".json"))
                {
                    finalPath = Path.Combine(path, "setting.json");
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(settingsToSave, options);

                if (File.Exists(finalPath))
                {
                    await Get_log.LoggerAsync("[INFO] [LoadAndSave.InputPathNotesJson] Файл существует, перезапись");
                    await File.WriteAllTextAsync(finalPath, json);
                    Console.WriteLine($"Настройки обновлены в: {finalPath}");
                }
                else
                {
                    await Get_log.LoggerAsync("[INFO] [LoadAndSave.InputPathNotesJson] Файл не существует, создание нового");
                    string directory = Path.GetDirectoryName(finalPath);
                    if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                    await File.WriteAllTextAsync(finalPath, json);
                    Console.WriteLine($"Новый файл настроек создан: {finalPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении по пути: {ex.Message}");
                await Get_log.LoggerAsync($"[ERROR] [LoadAndSave.InputPathNotesJson] Ошибка сохранения | exception='{ex.GetType().Name}', message='{ex.Message}', path='{path}'");
            }
            await Get_log.LoggerAsync("[DEBUG] [LoadAndSave.InputPathNotesJson] Выход из метода | result=успешно");
        }
    }
}