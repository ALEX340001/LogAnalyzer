using CommandLine;
using System.Collections.Generic;

namespace LogAnalyzer.Core.Services
{
    public class CommandLineOptions
    {
        [Option('a', "all-lines", Required = false, HelpText = "Выводить детальные строки.")]
        public bool PrintAllLine { get; set; } = false;

        [Option('r', "final-report", Required = false, HelpText = "Выводить итоговый отчёт.")]
        public bool PrintFinalReport { get; set; } = false;

        [Option('s', "show-console", Required = false, Default = true, HelpText = "Выводить лог в консоль (true/false).")]
        public bool ShowInConsole { get; set; } = true;

        [Option('d', "disable-logging", Required = false, HelpText = "Отключить логирование.")]
        public bool DisableAllLogging { get; set; } = false;

        [Option('o', "output", Required = false, HelpText = "Путь для сохранения статистики.")]
        public string OutputFilePath { get; set; } = null;

        [Option('k', "keywords", Required = false, HelpText = "Ключевые слова для поиска.")]
        public string Keywords { get; set; } = null;

        [Option('p', "path", Required = false, HelpText = "Путь к обрабатываемому лог-файлу или папке.")]
        public string InputFilePath { get; set; } = null;

        [Option('c', "config", Required = false, HelpText = "Путь к JSON файлу конфигурации.")]
        public string ConfigFilePath { get; set; } = null;

        [Option('f', "format", Required = false, Default = "txt", HelpText = "Формат выходного отчёта: txt, json, xml, wordgroup")]
        public string OutputFormat { get; set; } = "txt";

        // Дополнительные поля для десериализации из JSON (если понадобятся)
        public List<string> KeywordsList { get; set; } = null;
        public bool IncludeSubdirectories { get; set; } = true;
        public int MaxParallelFiles { get; set; } = 5;
    }
}