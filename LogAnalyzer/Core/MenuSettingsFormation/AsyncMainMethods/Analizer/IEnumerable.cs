// ------------------------------------------------------------
//  IEnumerable.cs 
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer.Statistics;

namespace LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer
{
    internal class IEnumerable
    {
        public static IEnumerable<string> GenerateReportLines(Statistics stats, bool includeDetails = false)
        {
            int fileIndex = 1;
            foreach (var fileKvp in stats.FileLineCounters)
            {
                string filePath = fileKvp.Key;
                yield return "";
                yield return "";
                yield return "";
                yield return "";
                yield return $"Файл {fileIndex}";
                yield return $"Файл: {filePath} | Строк с совпадениями: {fileKvp.Value}";
                if (stats.FileWordRegistry.TryGetValue(filePath, out var words) && words.Count > 0)
                {
                    foreach (var wordPair in words.OrderByDescending(p => p.Value))
                        yield return $"  слово [{wordPair.Key}] повторилось {wordPair.Value} раз(а)";
                }

                if (includeDetails && stats.FileMatchDetails.TryGetValue(filePath, out var details))
                {
                    yield return "  --- Найденные строки ---";
                    foreach (var d in details)
                        yield return $"  Строка {d.LineNumber}: [{string.Join(", ", d.FoundWords)}] {d.Line}";
                    yield return "";
                    yield return "";
                    yield return "";
                    yield return "";
                }

                fileIndex++;
            }
        }
    }
}