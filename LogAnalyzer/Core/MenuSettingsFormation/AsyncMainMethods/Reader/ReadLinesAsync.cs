// ================================================================
// ReadLinesAsync.cs
// ================================================================
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Reader
{
    internal class ReadLinesAsync
    {
        public static List<string> ListPathInLog = new List<string>();

        public static async IAsyncEnumerable<(string FileName, string PathInFile, int lineNumber)>
            AsyncReadLine(string explicitFilePath = null)
        {
            await Get_log.LoggerAsync($"[DEBUG] [ReadLinesAsync.AsyncReadLine] Вход в метод | explicitFilePath='{explicitFilePath ?? "null"}'");

            IEnumerable<string> filesToRead;
            if (!string.IsNullOrEmpty(explicitFilePath))
            {
                filesToRead = GetFilesFromPath(explicitFilePath);
            }
            else
            {
                if (ListPathInLog == null || ListPathInLog.Count == 0)
                {
                    Console.WriteLine("Предупреждение: список путей к логам пуст. Анализ не будет выполнен.");
                    await Get_log.LoggerAsync("[INFO] [ReadLinesAsync.AsyncReadLine] Список путей пуст, анализ пропущен");
                    yield break;
                }

                var allFiles = new List<string>();
                foreach (var path in ListPathInLog)
                {
                    allFiles.AddRange(GetFilesFromPath(path));
                }
                filesToRead = allFiles.Distinct();
            }

            await Get_log.LoggerAsync($"[INFO] [ReadLinesAsync.AsyncReadLine] Начинается чтение файлов | filesCount={filesToRead.Count()}");

            foreach (var filePath in filesToRead)
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Пропуск (файл не найден): {filePath}");
                    continue;
                }

                int lineNumber = 0;
                await foreach (string line in File.ReadLinesAsync(filePath))
                {
                    lineNumber++;
                    yield return (filePath, line, lineNumber);
                }
            }

            await Get_log.LoggerAsync("[DEBUG] [ReadLinesAsync.AsyncReadLine] Выход из метода | result=успешно");
        }

        private static IEnumerable<string> GetFilesFromPath(string path)
        {
            if (File.Exists(path))
            {
                yield return path;
            }
            else if (Directory.Exists(path))
            {
                var extensions = ChangingFilePath.MyExenshionList.Count > 0
                    ? ChangingFilePath.MyExenshionList
                    : new HashSet<string> { ".log", ".txt", ".json", ".xml" };
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase));
                foreach (var f in files)
                    yield return f;
            }
            else
            {
                Console.WriteLine($"Путь не существует: {path}");
            }
        }
    }
}