// ================================================================
// LogReportPrinter.cs 
// ================================================================
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer
{
    public class LogReportPrinter
    {
        public bool ShowInConsole { get; set; } = true;
        public bool DisableAllLogging { get; set; } = false;

        public async Task PrintSingleLineMatch(string path, int lineNumber, List<string> matchedWords, string line)
        {
            await Get_log.LoggerAsync($"[DEBUG] [LogReportPrinter.PrintSingleLineMatch] Вход в метод | path='{path}', lineNumber={lineNumber}, matchedWordsCount={matchedWords.Count}");
            if (DisableAllLogging) return;

            if (ShowInConsole) Console.WriteLine();
            string fileLine = $"Файл: {path}";
            if (ShowInConsole) Console.WriteLine(fileLine);
            string rowLine = $"Совпадение на строке: {lineNumber}";
            if (ShowInConsole) Console.WriteLine(rowLine);
            string wordsLine = $"Найденные слова: {string.Join(", ", matchedWords)}";
            if (ShowInConsole) Console.WriteLine(wordsLine);
            string contentLine = $"Строка: {line}";
            if (ShowInConsole) Console.WriteLine(contentLine);

            await Get_log.LoggerAsync($"[INFO] [LogReportPrinter.PrintSingleLineMatch] Обнаружено совпадение | file='{path}', lineNumber={lineNumber}, words='{string.Join(",", matchedWords)}'");
            await Get_log.LoggerAsync("[DEBUG] [LogReportPrinter.PrintSingleLineMatch] Выход из метода | result=успешно");
        }

        public async Task PrintFinalReport(Statistics stats)
        {
            await Get_log.LoggerAsync("[DEBUG] [LogReportPrinter.PrintFinalReport] Вход в метод | параметры: stats");
            if (DisableAllLogging) return;

            string mainHeader = "\n================ ИТОГОВАЯ СТАТИСТИКА ================";
            if (ShowInConsole) Console.WriteLine(mainHeader);
            await Get_log.LoggerAsync("[INFO] [LogReportPrinter.PrintFinalReport] Начало вывода итогового отчёта");
            foreach (var line in IEnumerable.GenerateReportLines(stats))
            {
                if (ShowInConsole) Console.WriteLine(line);
            }
            await Get_log.LoggerAsync("[DEBUG] [LogReportPrinter.PrintFinalReport] Выход из метода | result=успешно");
        }
    }
}