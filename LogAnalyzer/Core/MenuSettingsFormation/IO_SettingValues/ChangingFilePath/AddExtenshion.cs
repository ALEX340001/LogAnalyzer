// ================================================================
// AddExtenshion.cs 
// ================================================================
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath
{
    internal class AddExtenshion
    {
        public static async Task Call(HashSet<string> ValidExtensions, HashSet<string> MyExenshionList)
        {
            await Get_log.LoggerAsync("[DEBUG] [AddExtenshion.Call] Вход в метод | параметры: ValidExtensions, MyExenshionList");
            bool work = true;
            while (work)
            {
                Console.WriteLine("To exit, enter 2\n");
                string InputSearchExtenshion = Check.ReadLine($"Input extinshions for files: ");
                bool ResultearchExtenshion = ValidExtensions.Contains(InputSearchExtenshion);
                if (ResultearchExtenshion)
                {
                    MyExenshionList.Add(InputSearchExtenshion);
                    await Get_log.LoggerAsync($"[INFO] [AddExtenshion.Call] Добавлено расширение | extension='{InputSearchExtenshion}'");
                }
                int.TryParse(InputSearchExtenshion, out int InputUser_INT);
                if (InputUser_INT == 2)
                {
                    work = false;
                }
            }
            await Get_log.LoggerAsync("[DEBUG] [AddExtenshion.Call] Выход из метода | result=успешно");
        }
    }
}