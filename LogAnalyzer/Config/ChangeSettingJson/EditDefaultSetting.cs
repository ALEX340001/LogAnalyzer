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

namespace LogAnalyzer.Config.ChangeSettingJson
{
    internal class EditDefaultSetting
    {
        public static List<string> ListPathInLog = ChangingFilePath.ListPathInLog;

        public static void Call(LogAnalyzerSettings LinkSetting)
        {
            Console.Clear();
            Console.WriteLine("=== Editing Default Settings ===");
            Console.WriteLine("To leave a field unchanged, type 's kip' (with space).\n");

            // 1. Редактирование Name
            Console.WriteLine($"Current Name: '{LinkSetting.Name}'");
            string inputName = Check.ReadLine("Enter new Name (or type 's kip' to skip)");

            // Если ввели НЕ 's kip', то обновляем значение
            if (inputName.Trim().ToLower() != "s kip")
            {
                LinkSetting.Name = inputName;
                Console.WriteLine($"-> Name updated to: {LinkSetting.Name}");
            }
            else
            {
                Console.WriteLine("-> Skipped.");
            }
            Console.WriteLine();

            // 2. Редактирование IncludeSubdirectories
            Console.WriteLine($"Current Include Subdirectories: {LinkSetting.IncludeSubdirectories}");
            string inputSub = Check.ReadLine("Include subdirectories? (y/n / type 's kip' to skip)").Trim().ToLower();

            if (inputSub != "s kip")
            {
                LinkSetting.IncludeSubdirectories = (inputSub == "y" || inputSub == "yes");
                Console.WriteLine($"-> Updated to: {LinkSetting.IncludeSubdirectories}");
            }
            else
            {
                Console.WriteLine("-> Skipped.");
            }
            Console.WriteLine();

            // 3. Редактирование MaxParallelFiles
            Console.WriteLine($"Current Max Parallel Files: {LinkSetting.MaxParallelFiles}");
            string inputMaxFiles = Check.ReadLine("Enter new number (or type 's kip' to skip)");

            if (inputMaxFiles.Trim().ToLower() != "s kip")
            {
                if (int.TryParse(inputMaxFiles, out int maxFiles))
                {
                    LinkSetting.MaxParallelFiles = maxFiles;
                    Console.WriteLine($"-> Updated to: {LinkSetting.MaxParallelFiles}");
                }
                else
                {
                    Console.WriteLine("Warning: Invalid number format. Value not changed.");
                }
            }
            else
            {
                Console.WriteLine("-> Skipped.");
            }
            Console.WriteLine();

            // 4. Редактирование DefaultSavePath
            Console.WriteLine($"Current Default Save Path:\n{LinkSetting.DefaultSavePath}");
            string inputPath = Check.ReadLine("Enter new path (or type 's kip' to skip)");

            if (inputPath.Trim().ToLower() != "s kip")
            {
                LinkSetting.DefaultSavePath = inputPath;
                Console.WriteLine($"-> Path updated.");
            }
            else
            {
                Console.WriteLine("-> Skipped.");
            }
            Console.WriteLine("\n------------------------------------");

            // 5. Финальный опрос на сохранение всех измененных полей в JSON
            Console.WriteLine("Do you want to save ALL changes to JSON? (y/n)");
            string saveChoice = Console.ReadLine()?.ToLower();

            if (saveChoice == "y" || saveChoice == "yes")
            {
                string fileName = null;

                // Проверяем пути
                if (ListPathInLog != null && ListPathInLog.Count > 0)
                {
                    fileName = ListPathInLog.FirstOrDefault();
                }

                if (string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Save path in log is empty. Using default path...");
                    fileName = LinkSetting.SettingsFilePath;
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    string directory = Path.GetDirectoryName(fileName);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    LoadAndSave.SaveToJsonAsync(fileName, LinkSetting).GetAwaiter().GetResult();
                    Console.WriteLine($"Saved successfully to:\n{fileName}");
                }
                else
                {
                    Console.WriteLine("Error: Could not determine any save path.");
                }
                Task.Delay(2000).GetAwaiter().GetResult();
            }
        }
    }
}
