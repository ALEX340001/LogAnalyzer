using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Config.CreateAndEditConfig
{
    internal class SaveMenu
    {
        public static void CallSaveMenu(LogAnalyzerSettings LinkSetting)
        {
            string PathSaveFloaderConfig;
            string choise_str;
            string fullPath;
            Console.WriteLine("Enter 'edit' or 'e' to change the path, or press Enter to skip:");

            //List<string> linkPathSaveFloaderConfig = LinkSetting.PathSaveFloaderConfig;

            string? inputU = Console.ReadLine();

            // вовращаем пустой "null или флаг edit"
            choise_str = string.IsNullOrWhiteSpace(inputU) ? "null" : inputU;

            // Если ввели команду редактирования
            if (choise_str == "edit" || choise_str == "e")
            {
                PathSaveFloaderConfig = Check.ReadLine("Input path at config: ");

                string NameFileMyConfig = Check.ReadLine("Input please name file config");
                NameFileMyConfig = Path.ChangeExtension(NameFileMyConfig, ".json");

                fullPath = Path.Combine(PathSaveFloaderConfig, NameFileMyConfig);
             
                if (LinkSetting.PathSaveFloaderConfig == null)
                {
                    LinkSetting.PathSaveFloaderConfig = new List<string>();
                }

                LinkSetting.PathSaveFloaderConfig.Add(fullPath);



                LoadAndSave.SaveToJsonAsync(fullPath, LinkSetting).GetAwaiter().GetResult();

            }

            // Если пользователь просто нажал Enter (ввод определился как "null")
            if (choise_str == "null")
            {
                try
                {
                    fullPath = LinkSetting.PathSaveFloaderConfig.Any()
                          ? LinkSetting.PathSaveFloaderConfig.Last()
                          : LinkSetting.DefaultSavePath;

                    LoadAndSave.SaveToJsonAsync(fullPath, LinkSetting).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CallSaveMenu: {ex.Message}");
					Get_log.LoggerAsync($"CallSaveMenu: {ex.Message}");
                }
            }













        }
    }
}
