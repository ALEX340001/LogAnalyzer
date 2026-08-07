using LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer;
using LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Reader;
using LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Writer;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Data;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.Services
{
    public class AnalysisOrchestrator
    {
        private readonly LogAnalyzerSettings _settings;
        private readonly CommandLineOptions _cliOptions;

        public AnalysisOrchestrator(LogAnalyzerSettings settings, CommandLineOptions cliOptions)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _cliOptions = cliOptions ?? new CommandLineOptions();
        }

        public async Task RunAsync()
        {
            // 1. Собираем список файлов
            List<string> filePaths = new List<string>();
            if (!string.IsNullOrEmpty(_cliOptions.InputFilePath))
            {
                filePaths.Add(_cliOptions.InputFilePath);
            }
            else
            {
                filePaths = _settings.LogPaths ?? new List<string>();
            }

            if (!filePaths.Any())
            {
                await Get_log.LoggerAsync("[WARN] [AnalysisOrchestrator] Нет путей для анализа. Завершение.");
                return;
            }

            // 2. Группируем файлы (теперь каждый файл отдельно)
            var filesByFile = GroupFilesIndividually(filePaths);
            if (!filesByFile.Any())
            {
                await Get_log.LoggerAsync("[WARN] [AnalysisOrchestrator] Подходящих файлов не найдено.");
                return;
            }

            // 3. Определяем ключевые слова
            var wordsToSearch = ResolveKeywords();
            if (!wordsToSearch.Any())
            {
                await Get_log.LoggerAsync("[WARN] [AnalysisOrchestrator] Ключевых слов нет. Анализ отменён.");
                return;
            }

            // 4. Определяем базовую папку для сохранения
            string baseOutputDir = ResolveOutputDirectory();
            Directory.CreateDirectory(baseOutputDir);
            await Get_log.LoggerAsync($"[INFO] [AnalysisOrchestrator] Папка для сохранения результатов | dir='{baseOutputDir}'");

            // 5. Последовательно обрабатываем каждый файл
            int fileIndex = 0;
            foreach (var kvp in filesByFile)
            {
                fileIndex++;
                string filePath = kvp.Key;          // сам файл
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                var files = kvp.Value;              // список из одного элемента

                await Get_log.LoggerAsync($"[INFO] [AnalysisOrchestrator] Обработка файла | file='{fileName}', index={fileIndex}/{filesByFile.Count}");

                var appStats = new Statistics();
                var appPrinter = new LogReportPrinter
                {
                    ShowInConsole = _cliOptions.ShowInConsole,
                    DisableAllLogging = _cliOptions.DisableAllLogging
                };
                var processor = new LogStreamProcessor(appStats, appPrinter);

                // Генерируем поток
                async IAsyncEnumerable<(string, string, int)> GetSingleFileStream()
                {
                    await foreach (var entry in ReadLinesAsync.AsyncReadLine(filePath))
                        yield return entry;
                }

                await processor.AsyncAnalyzer(GetSingleFileStream(), wordsToSearch,
                    printAllLine: _cliOptions.PrintAllLine,
                    printFinalReport: _cliOptions.PrintFinalReport);

                // Формируем имя выходного файла
                string outputFile = Path.Combine(baseOutputDir,
                    $"{fileName}_analysis.{_cliOptions.OutputFormat}");

                await Writer.WriteInFile(appStats, outputFile, _cliOptions.OutputFormat);
                await Get_log.LoggerAsync($"[INFO] [AnalysisOrchestrator] Отчёт сохранён | file='{fileName}', output='{outputFile}'");
            }

            await Get_log.LoggerAsync("[INFO] [AnalysisOrchestrator] Анализ завершён.");
        }

        private string ResolveOutputDirectory()
        {
            // Приоритет: -o из командной строки, затем OutputFilePath из конфига, затем DefaultResultFolder
            if (!string.IsNullOrEmpty(_cliOptions.OutputFilePath))
                return _cliOptions.OutputFilePath;
            if (!string.IsNullOrEmpty(_settings.OutputFilePath))
                return _settings.OutputFilePath;
            return Writer.DefaultResultFolder;
        }

        private HashSet<string> ResolveKeywords()
        {
            var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (_cliOptions.KeywordsList?.Any() == true)
                words.UnionWith(_cliOptions.KeywordsList);
            else if (!string.IsNullOrEmpty(_cliOptions.Keywords))
                words.UnionWith(_cliOptions.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            else
                words.UnionWith(_settings.MyWordList ?? new HashSet<string>());
            return words;
        }

        public Dictionary<string, List<string>> GroupFilesIndividually(List<string> paths)
        {
            var result_ = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    // Ключ – путь к файлу, значение – список из одного этого файла
                    result_[path] = new List<string> { path };
                }
                else if (Directory.Exists(path))
                {
                    var extensions = ChangingFilePath.MyExenshionList.Count > 0
                        ? ChangingFilePath.MyExenshionList
                        : new HashSet<string> { ".log", ".txt", ".json", ".xml" };

                    var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                        .Where(f => extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase));

                    foreach (var file in files)
                    {
                        result_[file] = new List<string> { file };
                    }
                }
            }
            return result_;
        }

        private void AddToDict(Dictionary<string, List<string>> dict, string key, string file)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<string>();
            dict[key].Add(file);
        }
    }
}