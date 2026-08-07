using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Config.ChangeSettingJson.MenuEditWord;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;

namespace LogAnalyzer.Data
{

    public sealed class LogAnalyzerSettings
    {

        public Guid id { get; set; }
        public string Name { get; set; } 

        public List<string> Keywords { get; set; } = new();
        public bool IncludeSubdirectories { get; set; } = true;
        public int MaxParallelFiles { get; set; } = 5;

        public List<string> LogPaths { get; set; } = new();

        public List<string> PathSaveFloaderConfig { get; set; } 




        // поля хранящие пути
        public string DefaultSavePath { get; set; } = Path.Combine(Environment.GetFolderPath
            (Environment.SpecialFolder.UserProfile), "Documents", "MyAnalyzerLog");




        public string NotesFilePath => Path.Combine(DefaultSavePath, "notes.json"); // расширение заметок; путь сохраненеия по умолчанию заметок (расширение)





        // Путь к папке: Documents/MyAnalyzerLog/SettingJson
        public string SettingsFolderPath => Path.Combine(DefaultSavePath, "SettingJson");


        // 1. Добавляем приватное поле для хранения измененного пути
        private string _customSettingsFilePath;

        // 2. Переписываем свойство, чтобы у него появился блок set
        public string SettingsFilePath
        {
            get
            {
                // Если кастомный путь был установлен, возвращаем его. 
                // Иначе — собираем дефолтный путь по старой логике.
                return _customSettingsFilePath ?? Path.Combine(SettingsFolderPath, "setting.json");
            }
            set
            {
                _customSettingsFilePath = value;
            }
        }

        public static LogAnalyzerSettings Setting { get; internal set; } = new();



        // поля из других методов

        // Должно быть так:
        public HashSet<string> MyWordList { get; set; } = new HashSet<string>();


        //public static List<string> ListPathInLog = ChangingFilePath.ListPathInLog;



        public string OutputFilePath { get; set; } = string.Empty; // Путь для сохранения результатов анализа


        // поля из других методов


    }





}