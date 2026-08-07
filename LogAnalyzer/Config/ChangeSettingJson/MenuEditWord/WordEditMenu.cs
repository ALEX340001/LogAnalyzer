using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Config.ChangeSettingJson.MenuEditWord;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Core.NoteManager;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;

namespace LogAnalyzer.Config.ChangeSettingJson.MenuEditWord
{
    internal class WordEditMenu
    {

        public static void CallWordEditMenu(LogAnalyzerSettings LinkSetting)
        {
            Console.Clear();   // Очищаем

            string choise_str = Check.ReadLine("--- === MenuSettingsFormation === --- \n" +
               "1 - Print word list\n" +
               "2 - Get new word in list\n" +
               "3 - Delet word list \n" +
               "4 - Return main menu \n" +
               "");

            int.TryParse(choise_str, out int choise_int);
            switch (choise_int)
            {
                case 1:
                    {
                        PrintAllList.CallPrintAllList(LinkSetting);
                        break;
                    }
                case 2:
                    {
                        GetNewWord.CallGetNewWord(LinkSetting);
                        break;
                    }
                case 3:
                    {
                        DeletWordList.CallDeletWordList(LinkSetting);
                        break;
                    }
                case 4:
                    {
                        MenuSettingsFormation.MenuEditSetting();
                        break;
                    }
            }


        }
    }
}