using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Data;

namespace LogAnalyzer.Config.CreateAndEditConfig
{
    internal class DeletConfigs
    {
        public static void Call(LogAnalyzerSettings LinkSetting)
        {
            List<string> chosenPath = LinkSetting.PathSaveFloaderConfig ?? new List<string>();

            if (chosenPath.Count == 0)
            {
                Console.WriteLine("\n[INFO] History of configurations is empty. Nothing to delete.");
                Console.WriteLine("Press any key to return to Main Menu...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\n=== Delete Configuration File and Path ===");
            for (int i = 0; i < chosenPath.Count; i++)
            {
                string defaultMarker = (chosenPath[i] == LinkSetting.DefaultSavePath) ? " [Default]" : "";
                Console.WriteLine($"[{i + 1}] {chosenPath[i]}{defaultMarker}");
            }
            Console.WriteLine("[S] / [STOP] - Cancel and go back");

            while (true)
            {
                Console.Write($"\nEnter number to delete (1-{chosenPath.Count}) or 'stop': ");
                string? input = Console.ReadLine()?.Trim().ToLower();

                if (input == "stop" || input == "s")
                {
                    Console.WriteLine("Deletion canceled. Returning to Main Menu...");
                    return;
                }

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= chosenPath.Count)
                {
                    int targetIndex = choice - 1;
                    string pathToRemove = chosenPath[targetIndex];

                    // 1. ФИЗИЧЕСКОЕ УДАЛЕНИЕ ФАЙЛА С ДИСКА
                    try
                    {
                        if (File.Exists(pathToRemove))
                        {
                            File.Delete(pathToRemove);
                            Console.WriteLine($"\n[SUCCESS] File physically deleted from disk: {pathToRemove}");
                        }
                        else
                        {
                            Console.WriteLine($"\n[INFO] File was not found on disk (maybe already deleted), just removing from history.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Could not delete physical file: {ex.Message}");
                        Console.WriteLine("Proceeding to remove it from the program history anyway...");
                    }

                    // 2. УДАЛЕНИЕ ИЗ СПИСКА В ПАМЯТИ
                    LinkSetting.PathSaveFloaderConfig.RemoveAt(targetIndex);
                    Console.WriteLine($"Path removed from history.");

                    // Если удалили путь, который был дефолтным, сбрасываем его
                    if (pathToRemove == LinkSetting.DefaultSavePath)
                    {
                        LinkSetting.DefaultSavePath = string.Empty;
                        Console.WriteLine("[WARN] You deleted the default configuration file. Please set a new one later.");
                    }

                    // 3. СОХРАНЕНИЕ ОБНОВЛЕННОГО СПИСКА В ОСТАВШИЙСЯ КОНФИГ
                    try
                    {
                        string savePath = LinkSetting.PathSaveFloaderConfig.LastOrDefault() ?? LinkSetting.DefaultSavePath;

                        if (!string.IsNullOrWhiteSpace(savePath))
                        {
                            LoadAndSave.SaveToJsonAsync(savePath, LinkSetting).GetAwaiter().GetResult();
                            Console.WriteLine("Configuration history updated successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to update config history file: {ex.Message}");
                    }

                    Console.WriteLine("\nPress any key to return to Main Menu...");
                    Console.ReadKey(true);
                    return;
                }

                Console.WriteLine($"Invalid input. Type a number from 1 to {chosenPath.Count} or 'stop'.");
            }
        }
    }
}
