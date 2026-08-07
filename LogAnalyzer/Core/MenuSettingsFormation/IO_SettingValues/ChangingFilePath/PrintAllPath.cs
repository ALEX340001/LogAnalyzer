// ================================================================
//  PrintAllPath.cs (теперь async Task)
// ================================================================
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath
{
    internal class PrintAllPath
    {
        public static async Task Call(List<string> ListPathInLog)
        {
            await Get_log.LoggerAsync("[DEBUG] [PrintAllPath.Call] Вход в метод | параметры: ListPathInLog");
            if (ListPathInLog == null)
            {
                Console.WriteLine("Now in list = null \n Input path and try again ");
            }

            if (ListPathInLog != null)
            {
                int i = 1;
                foreach (var pathInFile in ListPathInLog)
                {
                    Console.WriteLine($"Path in file {i++}: {pathInFile}");
                }
                await Get_log.LoggerAsync($"[INFO] [PrintAllPath.Call] Вывод путей завершён | count={ListPathInLog.Count}");
            }
            await Get_log.LoggerAsync("[DEBUG] [PrintAllPath.Call] Выход из метода | result=успешно");
        }
    }
}