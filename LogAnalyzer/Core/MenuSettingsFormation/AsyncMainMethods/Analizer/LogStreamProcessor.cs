// ================================================================
// LogStreamProcessor.cs
// ================================================================
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer
{
    public class LogStreamProcessor
    {
        private readonly Statistics _stats;
        private readonly LogReportPrinter _printer;
        public LogStreamProcessor(Statistics stats, LogReportPrinter printer)
        {
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public async Task<Statistics> AsyncAnalyzer(
      IAsyncEnumerable<(string Path, string Line, int LineNumber)> source,
      HashSet<string> searchWords,
      bool printAllLine = false,
      bool printFinalReport = false)
        {
            var stopwatch = Stopwatch.StartNew();
            // Генерация CorrelationId, если его ещё нет (можно передавать извне)
            var correlationId = Guid.NewGuid().ToString("N");
            await Get_log.LoggerAsync($"[DEBUG] [LogStreamProcessor.AsyncAnalyzer] Вход в метод | correlationId={correlationId}, searchWordsCount={searchWords.Count}, printAllLine={printAllLine}, printFinalReport={printFinalReport}");

            // Храним текущий обрабатываемый файл для отслеживания прогресса
            string currentFilePath = null;
            int lineCounter = 0;

            try
            {
                await foreach (var (path, line, lineNumber) in source)
                {
                    // Определяем, начался ли новый файл
                    if (currentFilePath != path)
                    {
                        // Завершили предыдущий файл (если был)
                        if (currentFilePath != null)
                        {
                            await Get_log.LoggerAsync($"[INFO] [LogStreamProcessor.AsyncAnalyzer] Обработка файла завершена | file='{currentFilePath}', linesProcessed={lineCounter}, correlationId={correlationId}");
                        }
                        // Начинаем новый
                        currentFilePath = path;
                        lineCounter = 0;
                        await Get_log.LoggerAsync($"[INFO] [LogStreamProcessor.AsyncAnalyzer] Обработка файла начата | file='{currentFilePath}', correlationId={correlationId}");
                    }

                    lineCounter++;

                    // Обновляем счётчики
                    if (_stats.FileLineCounters.ContainsKey(path))
                        _stats.FileLineCounters[path]++;
                    else
                        _stats.FileLineCounters[path] = 1;

                    var matchedWords = searchWords
                        .Where(w => line.Contains(w, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    if (matchedWords.Count > 0)
                    {
                        // Инициализируем FileWordRegistry для файла, если нужно
                        if (!_stats.FileWordRegistry.ContainsKey(path))
                            _stats.FileWordRegistry[path] = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                        foreach (var word in matchedWords)
                        {
                            // FileWordRegistry: увеличиваем счётчик слова в этом файле
                            if (_stats.FileWordRegistry[path].ContainsKey(word))
                                _stats.FileWordRegistry[path][word]++;
                            else
                                _stats.FileWordRegistry[path][word] = 1;

                            // WordTotalCount: общий счётчик слова
                            if (_stats.WordTotalCount.ContainsKey(word))
                                _stats.WordTotalCount[word]++;
                            else
                                _stats.WordTotalCount[word] = 1;
                        }

                        // Заполняем MatchesByWord
                        foreach (var word in matchedWords)
                        {
                            if (!_stats.MatchesByWord.ContainsKey(word))
                                _stats.MatchesByWord[word] = new List<Statistics.MatchDetail>();
                            _stats.MatchesByWord[word].Add(new Statistics.MatchDetail
                            {
                                LineNumber = lineNumber,
                                Line = line,
                                FilePath = path,
                                FoundWords = matchedWords
                            });
                        }

                        if (matchedWords.Count > 0)
                        {
                            if (!_stats.MatchesByWord.ContainsKey(path))
                                _stats.MatchesByWord[path] = new List<Statistics.MatchDetail>();
                            _stats.MatchesByWord[path].Add(new Statistics.MatchDetail
                            {
                                LineNumber = lineNumber,
                                Line = line,
                                FilePath = path,
                                FoundWords = matchedWords
                            });

                            if (printAllLine)
                                await _printer.PrintSingleLineMatch(path, lineNumber, matchedWords, line);
                        }

                        // === Заполняем FileMatchDetails для детализированного отчёта ===
                        if (!_stats.FileMatchDetails.ContainsKey(path))
                            _stats.FileMatchDetails[path] = new List<Statistics.MatchDetail>();

                        _stats.FileMatchDetails[path].Add(new Statistics.MatchDetail
                        {
                            LineNumber = lineNumber,
                            Line = line,
                            FilePath = path,
                            FoundWords = matchedWords
                        });

                        // Каждые 10 000 строк выводим прогресс, чтобы не забивать лог
                        if (lineCounter % 10000 == 0)
                        {
                            await Get_log.LoggerAsync($"[INFO] [LogStreamProcessor.AsyncAnalyzer] Прогресс обработки файла | file='{path}', linesProcessed={lineCounter}, correlationId={correlationId}");
                        }
                    }

                    // Логируем последний обработанный файл (если был)
                    if (currentFilePath != null)
                    {
                        await Get_log.LoggerAsync($"[INFO] [LogStreamProcessor.AsyncAnalyzer] Обработка файла завершена | file='{currentFilePath}', linesProcessed={lineCounter}, correlationId={correlationId}");
                    }
                }
            }
            catch (Exception ex)
            {
                await Get_log.LoggerAsync($"[ERROR] [LogStreamProcessor.AsyncAnalyzer] Ошибка обработки потока | exception='{ex.GetType().Name}', message='{ex.Message}', file='{currentFilePath}', lineNumber={lineCounter}, correlationId={correlationId}");
                throw;
            }

            stopwatch.Stop();
            int totalFiles = _stats.FileLineCounters.Count;
            int totalLines = _stats.FileLineCounters.Sum(x => x.Value);
            int totalMatches = _stats.MatchesByWord.Sum(x => x.Value.Count);
            await Get_log.LoggerAsync($"[INFO] [LogStreamProcessor.AsyncAnalyzer] Анализ завершён | correlationId={correlationId}, files={totalFiles}, totalLines={totalLines}, totalMatches={totalMatches}, elapsedMs={stopwatch.ElapsedMilliseconds}");

            if (printFinalReport)
                await _printer.PrintFinalReport(_stats);

            await Get_log.LoggerAsync("[DEBUG] [LogStreamProcessor.AsyncAnalyzer] Выход из метода | result=Statistics");
            return _stats;
        }
    }
}