using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;

namespace LogAnalyzer.Config.ChangeSettingJson.MenuEditWord
{
    internal class DeletWordList
    {
        public static void CallDeletWordList(LogAnalyzerSettings LinkSetting)
        {
            bool work = true;
            while(work)
            {

                int i = 0;

                foreach (var item in LinkSetting.MyWordList)
                {
                    Console.WriteLine($"item {i}: {item}");
                    i++;
                }
                string choiseUser = Check.ReadLine("Input value word at this need delet: ");
                Console.WriteLine("To exit, inpu 'exit' or 'e'");
                if (choiseUser == "exit" || choiseUser == "e")
                {
                    work = false;
                }
                // 1. Безопасно переводим ввод пользователя в число
                if (int.TryParse(choiseUser, out int index) && index >= 0 && index < LinkSetting.MyWordList.Count)
                {
                    // 2. Достаем элемент по индексу
                    string itemToRemove = LinkSetting.MyWordList.ElementAt(index);

                    // 3. Удаляем конкретно это значение
                    LinkSetting.MyWordList.Remove(itemToRemove);
                    Console.WriteLine($"Element {choiseUser} deleted !");
                }
            }
           

        }

    }
}