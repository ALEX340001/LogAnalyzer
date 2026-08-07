// ================================================================
// SearchPath.cs 
// ================================================================
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath
{
    internal class SearchPath
    {
        public static async Task Call(string userPath, List<string> ListPathInLog, HashSet<string> MyExenshionList)
        {
            await Get_log.LoggerAsync($"[DEBUG] [SearchPath.Call] Вход в метод | userPath='{userPath}'");

            if (File.Exists(userPath))
            {
                string ext = Path.GetExtension(userPath);
                if (MyExenshionList.Contains(ext))
                {
                    if (!ListPathInLog.Contains(userPath))
                    {
                        ListPathInLog.Add(userPath);
                        Console.WriteLine($"Добавлен файл: {userPath}");
                        await Get_log.LoggerAsync($"[INFO] [SearchPath.Call] Файл добавлен в список | path='{userPath}'");
                    }
                    else
                    {
                        Console.WriteLine("Файл уже есть в списке.");
                    }
                }
                else
                {
                    Console.WriteLine($"Расширение '{ext}' не поддерживается. Сначала добавьте его через пункт 1.");
                }
                await Get_log.LoggerAsync("[DEBUG] [SearchPath.Call] Выход из метода | result=успешно");
                return;
            }

            string directory = Directory.Exists(userPath) ? userPath : Path.GetDirectoryName(userPath);
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                Console.WriteLine("Указанная папка не существует.");
                await Get_log.LoggerAsync("[DEBUG] [SearchPath.Call] Выход из метода | result=папка не существует");
                return;
            }

            var files = Directory.GetFiles(directory)
                .Where(file => MyExenshionList.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (files.Count == 0)
            {
                Console.WriteLine("В указанной папке нет файлов с поддерживаемыми расширениями.");
                await Get_log.LoggerAsync("[DEBUG] [SearchPath.Call] Выход из метода | result=нет подходящих файлов");
                return;
            }

            Console.WriteLine($"Найдено файлов: {files.Count}. Добавляем все...");
            int added = 0;
            foreach (var file in files)
            {
                if (!ListPathInLog.Contains(file))
                {
                    ListPathInLog.Add(file);
                    added++;
                }
            }
            await Get_log.LoggerAsync($"[INFO] [SearchPath.Call] Добавлены файлы в список | count={added}");
            Console.WriteLine($"Добавлено {added} новых файлов.");
            await Get_log.LoggerAsync("[DEBUG] [SearchPath.Call] Выход из метода | result=успешно");
        }
    }
}