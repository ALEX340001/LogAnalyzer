using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Data;

namespace LogAnalyzer.Config.CreateAndEditConfig
{
    internal class ChoiseConfigInDefault
    {
        public static void Call(LogAnalyzerSettings LinkSetting)
        {

            // 1. Извлекаем список всех доступных путей в переменную, как вы и хотели
            List<string> chosenPath = LinkSetting.PathSaveFloaderConfig ?? new List<string>();

            // Если путей для выбора нет вообще
            if (chosenPath.Count == 0)
            {
                Console.WriteLine("\n[INFO] History of configurations is empty. Nothing to set as default.");
                Console.WriteLine("Press any key to return to Main Menu...");
                Console.ReadKey(true);
                return; // Возврат в главное меню
            }

            // 2. Отображаем пути пользователю
            Console.WriteLine("\n=== Select Default Configuration ===");
            for (int i = 0; i < chosenPath.Count; i++)
            {
                // Помечаем звездочкой текущий дефолтный путь для наглядности
                string currentMarker = (chosenPath[i] == LinkSetting.DefaultSavePath) ? " *Active*" : "";
                Console.WriteLine($"[{i + 1}] {chosenPath[i]}{currentMarker}");
            }
            Console.WriteLine("[S] / [STOP] - Back to Main Menu");

            // 3. Бесконечный цикл опроса ввода
            while (true)
            {
                Console.Write($"\nEnter number (1-{chosenPath.Count}) to set default, or 'stop': ");
                string? input = Console.ReadLine()?.Trim().ToLower();

                // Проверка на команду возврата
                if (input == "stop" || input == "s")
                {
                    Console.WriteLine("Returning to Main Menu...");
                    return; // Просто выходим из метода, управление возвращается в главное меню
                }

                // Проверка на ввод корректного номера
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= chosenPath.Count)
                {
                    int targetIndex = choice - 1;
                    string selectedPath = chosenPath[targetIndex];

                    // 4. Перезаписываем путь по умолчанию в самом объекте настроек
                    LinkSetting.DefaultSavePath = selectedPath;
                    Console.WriteLine($"\nSuccess! New default path set to: {selectedPath}");

                    // 5. Сразу сохраняем изменения настроек в JSON на диск, чтобы они не стерлись при перезапуске
                    try
                    {
                        // Используем последний активный путь для перезаписи файла настроек
                        string savePath = LinkSetting.PathSaveFloaderConfig.LastOrDefault() ?? selectedPath;
                        LoadAndSave.SaveToJsonAsync(savePath, LinkSetting).GetAwaiter().GetResult();
                        Console.WriteLine("Settings successfully auto-saved to disk.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to auto-save settings: {ex.Message}");
                    }

                    Console.WriteLine("\nPress any key to return to Main Menu...");
                    Console.ReadKey(true);
                    return; // Успешно завершили и вышли в главное меню
                }

                Console.WriteLine($"Invalid input. Type a number from 1 to {chosenPath.Count} or 'stop'.");
            }
        }

    }
}
