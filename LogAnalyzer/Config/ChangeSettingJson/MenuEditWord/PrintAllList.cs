using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Data;

namespace LogAnalyzer.Config.ChangeSettingJson.MenuEditWord
{
    internal class PrintAllList
    {

        public static void CallPrintAllList(LogAnalyzerSettings LinkSetting)
        {
            int i = 1;
            foreach(var item in LinkSetting.MyWordList)
            {
                Console.WriteLine($"item {i}: {item}");
                i++;
            }

        }

    }
}
