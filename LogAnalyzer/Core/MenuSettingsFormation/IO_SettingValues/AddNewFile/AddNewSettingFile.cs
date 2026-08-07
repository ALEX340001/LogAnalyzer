// ================================================================
// AddNewSettingFile.cs
// ================================================================
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.NoteManager.GetDataAndInputNotes.AddNewNotes
{
    internal class AddNewSettingFile
    {
        public static LogAnalyzerSettings settingsToSave = LogAnalyzerSettings.Setting;

        public static async Task InputDateInFile()
        {
            await Get_log.LoggerAsync("[DEBUG] [AddNewSettingFile.InputDateInFile] Вход в метод | параметры: отсутствуют");
            Console.Clear();
            string date_str = Check.ReadLine($"Input date in from format  :");
            await Get_log.LoggerAsync($"[INFO] [AddNewSettingFile.InputDateInFile] Дата введена | date_str='{date_str}'");
            await Get_log.LoggerAsync("[DEBUG] [AddNewSettingFile.InputDateInFile] Выход из метода | result=успешно");
        }

        public static async Task GetDateInFile()
        {
            await Get_log.LoggerAsync("[DEBUG] [AddNewSettingFile.GetDateInFile] Вход в метод | параметры: отсутствуют");
            Console.Clear();
            var settingsToSave = LogAnalyzerSettings.Setting;
            await LoadAndSave.SaveToJsonAsync(settingsToSave.SettingsFilePath, settingsToSave);
            Console.WriteLine("Настройки успешно сохранены в " + settingsToSave.SettingsFilePath);
            await Get_log.LoggerAsync($"[INFO] [AddNewSettingFile.GetDateInFile] Настройки сохранены | filePath='{settingsToSave.SettingsFilePath}'");
            await Get_log.LoggerAsync("[DEBUG] [AddNewSettingFile.GetDateInFile] Выход из метода | result=успешно");
        }

        public static async Task GetDetails()
        {
            await Get_log.LoggerAsync("[DEBUG] [AddNewSettingFile.GetDetails] Вход в метод | параметры: отсутствуют");
            Console.Clear();
            var Setting_ = LogAnalyzerSettings.Setting;
            Console.Write($"Количество потоков (сейчас {LogAnalyzerSettings.Setting.MaxParallelFiles}): ");
            if (int.TryParse(Console.ReadLine(), out int threads))
            {
                Setting_.MaxParallelFiles = threads;
            }
            Console.Write($"Путь сохранения (сейчас {Setting_.DefaultSavePath}): ");
            string newPath = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                Setting_.DefaultSavePath = newPath;
            }
            await GetDateInFile();
            await Get_log.LoggerAsync($"[INFO] [AddNewSettingFile.GetDetails] Настройки обновлены | threads={threads}, path='{newPath}'");
            await Get_log.LoggerAsync("[DEBUG] [AddNewSettingFile.GetDetails] Выход из метода | result=успешно");
        }
    }
}