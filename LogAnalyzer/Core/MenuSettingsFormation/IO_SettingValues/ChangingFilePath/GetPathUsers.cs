// ================================================================
// GetPathUsers.cs (теперь async Task)
// ================================================================
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath
{
    internal class GetPathUsers
    {
        public static async Task Call(List<string> ListPathInLog, HashSet<string> MyExenshionList)
        {
            await Get_log.LoggerAsync("[DEBUG] [GetPathUsers.Call] Вход в метод | параметры: ListPathInLog, MyExenshionList");
            string PathToFile = Check.ReadLine("");
            SearchPath.Call(PathToFile, ListPathInLog, MyExenshionList);
            await Get_log.LoggerAsync("[DEBUG] [GetPathUsers.Call] Выход из метода | result=успешно");
        }
    }
}