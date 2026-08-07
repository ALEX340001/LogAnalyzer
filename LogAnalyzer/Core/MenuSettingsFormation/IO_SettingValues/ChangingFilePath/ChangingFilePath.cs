// ================================================================
//  ChangingFilePath.cs (теперь async Task)
// ================================================================
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath
{
    internal class ChangingFilePath
    {
        public static List<string> ListPathInLog = new List<string>();
        public static readonly HashSet<string> MyExenshionList = new(StringComparer.OrdinalIgnoreCase);
        public static readonly HashSet<string> ValidExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".log", ".txt", ".json", ".xml", ".out", ".err", ".csv", ".tsv", ".syslog", ".trace", ".event"
        };

        public static async Task GetPath()
        {
            await Get_log.LoggerAsync("[DEBUG] [ChangingFilePath.GetPath] Вход в метод | параметры: отсутствуют");
            bool work = true;
            while (work)
            {
                Console.Clear();
                string ChoiseUser = Check.ReadLine(
                    " - - - Menu serch and get extenshions - - - \n\n" +
                    " 1 - Add more extensions for files\n" +
                    " 2 - Please input path to file \n" +
                    " 3 - print all path in console \n" +
                    " 4 - Return to menu \n" +
                    " \n help - What and how the program expects");
                int.TryParse(ChoiseUser, out int ChoiseUser_int);

                if (ChoiseUser == "help")
                {
                    Console.Clear();
                    Console.WriteLine($"\n1 - First, add extensions\r\n2 - Then, add file links\r\n3 - Check and print\r\n4 - Exit\r\nhelp - help - help \n");
                    Console.WriteLine("Input key that continue");
                    Console.ReadKey();
                    continue;
                }

                await Get_log.LoggerAsync($"[INFO] [ChangingFilePath.GetPath] Пользователь выбрал пункт меню | choice='{ChoiseUser}'");

                switch (ChoiseUser_int)
                {
                    case 1:
                        await AddExtenshion.Call(ValidExtensions, MyExenshionList);
                        break;
                    case 2:
                        GetPathUsers.Call(ListPathInLog, MyExenshionList);
                        break;
                    case 3:
                        PrintAllPath.Call(ListPathInLog);
                        break;
                    case 4:
                        work = false;
                        break;
                }
            }
            await Get_log.LoggerAsync("[DEBUG] [ChangingFilePath.GetPath] Выход из метода | result=успешно");
        }
    }
}