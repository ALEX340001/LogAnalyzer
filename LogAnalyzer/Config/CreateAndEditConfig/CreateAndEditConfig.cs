using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Core.NoteManager;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;

namespace LogAnalyzer.Config.CreateAndEditConfig
{
    internal class CreateAndEditConfig
    {
        public static void MainMenuCreateAndEditConfig(LogAnalyzerSettings LinkSetting)
        {
            string choise_str = Check.ReadLine("--- === MenuSettingsFormation === --- \n" +

               "1 - Print all config \n" +          // вывод всех конфигураций
               "2 - Save at config \n" +            // Сохранить текущую конфигурацию
               "3 - Select default configuration\n" +            // Выбрать конфигурацию
               "4 - Delet configs \n" +             // Удалить кофнигурацию
               "5 - exit \n" +           

               "help 'h' -  if you're confused \n"

               );  






            int.TryParse(choise_str, out int choise_int);
            if (choise_str == "help"|| choise_str == "h")
            {
                Console.WriteLine("\n==================================================");
                Console.WriteLine("                  LOG ANALYZER HELP               ");
                Console.WriteLine("==================================================");
                Console.WriteLine("Available commands at prompts:");
                Console.WriteLine("  edit, e        - Change the configuration folder and file name");
                Console.WriteLine("  stop, s        - Cancel the current operation and go back");
                Console.WriteLine("  help, h, -help - Show this reference guide");
                Console.WriteLine();
                Console.WriteLine("Main Menu Features:");

                Console.WriteLine("  1. Save Config   - Saves current settings. Press Enter to use last path.");

                Console.WriteLine("  2. Set Default   - Choose which config loads automatically by default.");

                Console.WriteLine("  3. Delete Config - Physically deletes a JSON file and clears its history.");

                Console.WriteLine("==================================================");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
            }

            switch (choise_int)
            {
                case 1:
                    {
                        PrintAlLFileConfig.SelectConfigFromHistory(LinkSetting);
                        break;
                    }
                case 2:
                    {
                        SaveMenu.CallSaveMenu(LinkSetting);
                        break;
                    }
                case 3:
                    {
                        ChoiseConfigInDefault.Call(LinkSetting);
                        break;
                    }
                case 4:
                    {
                        DeletConfigs.Call(LinkSetting);
                        break;
                    }
                case 5:
                    {
                        MenuSettingsFormation.MenuEditSetting();
                        break;
                    }







            }





        }
    }
}