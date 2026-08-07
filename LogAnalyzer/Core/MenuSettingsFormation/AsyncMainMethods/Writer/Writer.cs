// ================================================================
// Writer.cs
// ================================================================
using LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Writer
{
    public class Writer
    {
        internal static string DefaultResultFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "ResultFilter");


        public static async Task WriteInFile(Statistics stats, string outputPath = null, string format = "txt")
        {
            await Get_log.LoggerAsync($"[DEBUG] [Writer.WriteInFile] Вход в метод | outputPath='{outputPath ?? "null"}', format='{format}'");

            string targetFile = outputPath ?? Path.Combine(DefaultResultFolder, $"{DateTime.Now:yyyy-MM-dd}_log.txt");
            if (Directory.Exists(targetFile))
            {
                string ext = format.ToLower() switch
                {
                    "json" => ".json",
                    "xml" => ".xml",
                    _ => ".txt"
                };
                targetFile = Path.Combine(targetFile, $"{DateTime.Now:yyyy-MM-dd}_log{ext}");
            }
            else
            {
                string desiredExt = format.ToLower() switch
                {
                    "json" => ".json",
                    "xml" => ".xml",
                    _ => ".txt"
                };
                if (!string.IsNullOrEmpty(Path.GetExtension(targetFile)))
                {
                    string currentExt = Path.GetExtension(targetFile);
                    if (!currentExt.Equals(desiredExt, StringComparison.OrdinalIgnoreCase))
                        targetFile = Path.ChangeExtension(targetFile, desiredExt);
                }
                else targetFile += desiredExt;
            }

            string directory = Path.GetDirectoryName(targetFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                await Get_log.LoggerAsync($"[INFO] [Writer.WriteInFile] Создана папка для отчёта | directory='{directory}'");
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"[Сохранение] Файл отчёта ({format.ToUpper()}): {targetFile}");
            Console.ResetColor();

            switch (format.ToLower())
            {
                case "json":
                    await WriteJsonReport(stats, targetFile);
                    break;
                case "xml":
                    await WriteXmlReport(stats, targetFile);
                    break;
                case "wordgroup":
                    await WriteGroupedByWordReport(stats, targetFile);
                    break;
                default:
                    await WriteTextReport(stats, targetFile);
                    break;
            }

            await Get_log.LoggerAsync("[DEBUG] [Writer.WriteInFile] Выход из метода | result=успешно");
        }

        private static async Task WriteTextReport(Statistics stats, string filePath)
        {
            await Get_log.LoggerAsync($"[DEBUG] [Writer.WriteTextReport] Вход в метод | filePath='{filePath}'");
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                await writer.WriteLineAsync("\n================ ИТОГОВАЯ СТАТИСТИКА ================");
                foreach (var line in IEnumerable.GenerateReportLines(stats, includeDetails: true))
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    await writer.WriteLineAsync(line);
                }
            }
            await Get_log.LoggerAsync("[DEBUG] [Writer.WriteTextReport] Выход из метода | result=успешно");
        }

        private static async Task WriteJsonReport(Statistics stats, string filePath)
        {
            await Get_log.LoggerAsync($"[DEBUG] [Writer.WriteJsonReport] Вход в метод | filePath='{filePath}'");
            var report = new
            {
                GeneratedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                FileLineCounters = stats.FileLineCounters,
                WordTotalCount = stats.WordTotalCount,
                FileWordRegistry = stats.FileWordRegistry.ToDictionary(f => f.Key, f => f.Value.ToDictionary(w => w.Key, w => w.Value)),
                FileMatchDetails = stats.FileMatchDetails.ToDictionary(f => f.Key, f => f.Value.Select(d => new {
                    LineNumber = d.LineNumber,
                    Line = d.Line,
                    FoundWords = d.FoundWords
                }).ToList())
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(report, options);
            await File.WriteAllTextAsync(filePath, json);
            await Get_log.LoggerAsync("[DEBUG] [Writer.WriteJsonReport] Выход из метода | result=успешно");
        }

        private static async Task WriteXmlReport(Statistics stats, string filePath)
        {
            await Get_log.LoggerAsync($"[DEBUG] [Writer.WriteXmlReport] Вход в метод | filePath='{filePath}'");
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<Report>");
            sb.AppendLine($"  <GeneratedAt>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</GeneratedAt>");
            sb.AppendLine("  <FileStatistics>");
            foreach (var fileKvp in stats.FileLineCounters)
            {
                string filePathEsc = System.Net.WebUtility.HtmlEncode(fileKvp.Key);
                sb.AppendLine($"    <File path=\"{filePathEsc}\" totalLines=\"{fileKvp.Value}\">");
                if (stats.FileWordRegistry.TryGetValue(fileKvp.Key, out var words))
                {
                    foreach (var wordKvp in words)
                        sb.AppendLine($"      <Word name=\"{System.Net.WebUtility.HtmlEncode(wordKvp.Key)}\" count=\"{wordKvp.Value}\"/>");
                }
                if (stats.FileMatchDetails.TryGetValue(fileKvp.Key, out var details))
                {
                    sb.AppendLine("      <Matches>");
                    foreach (var d in details)
                    {
                        sb.AppendLine($"        <Match lineNumber=\"{d.LineNumber}\">");
                        sb.AppendLine($"          <FoundWords>{System.Net.WebUtility.HtmlEncode(string.Join(", ", d.FoundWords))}</FoundWords>");
                        sb.AppendLine($"          <Line>{System.Net.WebUtility.HtmlEncode(d.Line)}</Line>");
                        sb.AppendLine("        </Match>");
                    }
                    sb.AppendLine("      </Matches>");
                }
                sb.AppendLine("    </File>");
            }
            sb.AppendLine("  </FileStatistics>");
            sb.AppendLine("  <WordTotalCount>");
            foreach (var wordKvp in stats.WordTotalCount)
                sb.AppendLine($"    <Word name=\"{System.Net.WebUtility.HtmlEncode(wordKvp.Key)}\" totalCount=\"{wordKvp.Value}\"/>");
            sb.AppendLine("  </WordTotalCount>");
            sb.AppendLine("</Report>");
            await File.WriteAllTextAsync(filePath, sb.ToString());
            await Get_log.LoggerAsync("[DEBUG] [Writer.WriteXmlReport] Выход из метода | result=успешно");
        }

        private static async Task WriteGroupedByWordReport(Statistics stats, string filePath)
        {
            await Get_log.LoggerAsync($"[DEBUG] [Writer.WriteGroupedByWordReport] Вход в метод | filePath='{filePath}'");
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await writer.WriteLineAsync("================ ОТЧЁТ ПО КЛЮЧЕВЫМ СЛОВАМ ================");
                await writer.WriteLineAsync();
                foreach (var wordEntry in stats.MatchesByWord.OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
                {
                    string word = wordEntry.Key;
                    var details = wordEntry.Value;
                    int totalOccurrences = details.Count;
                    var uniqueFiles = details.Select(d => d.FilePath).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                    await writer.WriteLineAsync($"Ключевое слово: \"{word}\"");
                    await writer.WriteLineAsync($" Статистика: найдено {totalOccurrences} раз в {uniqueFiles} файлах");
                    await writer.WriteLineAsync();
                    var byFile = details.GroupBy(d => d.FilePath, StringComparer.OrdinalIgnoreCase);
                    foreach (var fileGroup in byFile.OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        string file = fileGroup.Key;
                        await writer.WriteLineAsync($" Файл: {file}");
                        foreach (var match in fileGroup.OrderBy(m => m.LineNumber))
                        {
                            await writer.WriteLineAsync($" Строка {match.LineNumber}: {match.Line.Trim()}");
                        }
                        await writer.WriteLineAsync();
                    }
                    await writer.WriteLineAsync(new string('-', 60));
                    await writer.WriteLineAsync();
                }
                await writer.WriteLineAsync("================ ИТОГОВАЯ СТАТИСТИКА ПО ВСЕМ СЛОВАМ ================");
                foreach (var kv in stats.WordTotalCount.OrderByDescending(kv => kv.Value))
                {
                    await writer.WriteLineAsync($"Слово \"{kv.Key}\" было найдено {kv.Value} раз(а)");
                }
                await writer.WriteLineAsync("================================================================");
            }
            await Get_log.LoggerAsync("[DEBUG] [Writer.WriteGroupedByWordReport] Выход из метода | result=успешно");
        }
        // Внутри класса Writer (файл Writer.cs)
        public static async Task WriteDetailedReportForSingleFile(Statistics stats, string outputPath)
        {
            await Get_log.LoggerAsync($"[DEBUG] [Writer.WriteDetailedReportForSingleFile] Вход в метод | outputPath='{outputPath}'");

            // Убедимся, что папка существует
            string dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                // Генерируем строки с включённой детализацией
                foreach (var line in IEnumerable.GenerateReportLines(stats, includeDetails: true))
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        // Дополнительный отступ – 5 пустых строк, как просили (но обычно хватит 1-2)
                        await writer.WriteLineAsync();
                        await writer.WriteLineAsync();
                        continue;
                    }
                    await writer.WriteLineAsync(line);
                }
            }

            await Get_log.LoggerAsync("[DEBUG] [Writer.WriteDetailedReportForSingleFile] Выход из метода | result=успешно");
        }
    }
}