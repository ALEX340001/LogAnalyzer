// ================================================================
// 1. MenuSettingsFormation.cs
// ================================================================
using LogAnalyzer.Config.ChangeSettingJson;
using LogAnalyzer.Config.ChangeSettingJson.MenuEditWord;
using LogAnalyzer.Config.CreateAndEditConfig;
using LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer;
using LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Reader;
using LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Writer;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Core.NoteManager.GetDataAndInputNotes.AddNewNotes;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;
using MyNotes.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogAnalyzer;

namespace LogAnalyzer.Core.NoteManager
{
    internal class MenuSettingsFormation
    {
        public static LogAnalyzerSettings LinkSetting = LogAnalyzerSettings.Setting;

        public static async Task MenuEditSetting()
        {
            await Get_log.LoggerAsync("[DEBUG] [MenuSettingsFormation.MenuEditSetting] Вход в метод | параметры: отсутствуют");

            var LogAnalyzerSettings = await LoadAndSave.LoadAndSaveAsync(LinkSetting.SettingsFilePath);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            bool work = true;

            while (work)
            {
                Console.Clear();
                MenuConsoleHelper.ShowConfigBlock();
                var menuText = new System.Text.StringBuilder();
                menuText.AppendLine("--- === MenuSettingsFormation === ---");
                menuText.AppendLine("-------------------------------------------------------");
                menuText.AppendLine("1 - Forming paths to folders");
                menuText.AppendLine("2 - Enter the number of files to be processed simultaneously");
                menuText.AppendLine("3 - Menu edit word");
                menuText.AppendLine("4 - Changing default settings");
                menuText.AppendLine("5 - Create and save config");
                menuText.AppendLine("6 - test_read_sync");
                menuText.AppendLine("-------------------------------------------------------");
                menuText.AppendLine("7 - Exit");
                menuText.AppendLine("-------------------------------------------------------");

                Console.ForegroundColor = ConsoleColor.Yellow;
                menuText.Append("-> Selected action: ");
                Console.ForegroundColor = ConsoleColor.White;

                string choise_str = Check.ReadLine(menuText.ToString());
                int.TryParse(choise_str, out int choise_int);

                await Get_log.LoggerAsync($"[INFO] [MenuSettingsFormation.MenuEditSetting] Пользователь выбрал действие | choice='{choise_str}'");

                switch (choise_int)
                {
                    case 1:
                        ChangingFilePath.GetPath();
                        break;
                    case 2:
                        try
                        {
                            string userInput = Check.ReadLine("");
                            if (int.TryParse(userInput, out int newValue))
                            {
                                LinkSetting.MaxParallelFiles = newValue;
                                await Get_log.LoggerAsync($"[INFO] [MenuSettingsFormation.MenuEditSetting] Параметр MaxParallelFiles обновлён | newValue={newValue}");
                                await LoadAndSave.SaveToJsonAsync(LinkSetting.SettingsFilePath, LinkSetting);
                                Console.WriteLine("Настройки обновлены.");
                            }
                            else
                            {
                                Console.WriteLine("Ошибка: введено не целое число или некорректный формат.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"MenuEditSetting case 2: {ex.Message}");
                            await Get_log.LoggerAsync($"[ERROR] [MenuSettingsFormation.MenuEditSetting] Ошибка изменения параметра | exception='{ex.GetType().Name}', message='{ex.Message}', choice='2'");
                        }
                        break;
                    case 3:
                        WordEditMenu.CallWordEditMenu(LogAnalyzerSettings);
                        break;
                    case 4:
                        EditDefaultSetting.Call(LinkSetting);
                        break;
                    case 5:
                        CreateAndEditConfig.MainMenuCreateAndEditConfig(LinkSetting);
                        Console.WriteLine("in processing");
                        break;
                    // В MenuEditSetting, внутри case 6 (тестовый анализ)
                    case 6:
                        {
                            await Get_log.LoggerAsync($"[INFO] [MenuSettingsFormation.MenuEditSetting] Старт анализа каждого файла отдельно | pathsCount={ReadLinesAsync.ListPathInLog.Count}");

                            // Если список пуст, предупреждаем и выходим
                            if (ReadLinesAsync.ListPathInLog == null || ReadLinesAsync.ListPathInLog.Count == 0)
                            {
                                await Get_log.LoggerAsync("[WARN] [MenuSettingsFormation.MenuEditSetting] Нет файлов для анализа");
                                break;
                            }

                            // Определяем базовую папку для сохранения отчётов из настроек
                            string baseOutputDir = Path.Combine(LinkSetting.DefaultSavePath, "Reports");
                            Directory.CreateDirectory(baseOutputDir);
                            await Get_log.LoggerAsync($"[INFO] [MenuSettingsFormation.MenuEditSetting] Папка для отчётов | path='{baseOutputDir}'");

                            int fileIndex = 0;
                            foreach (var filePath in ReadLinesAsync.ListPathInLog)
                            {
                                if (!File.Exists(filePath))
                                {
                                    await Get_log.LoggerAsync($"[WARN] [MenuSettingsFormation.MenuEditSetting] Пропущен несуществующий файл | path='{filePath}'");
                                    continue;
                                }

                                fileIndex++;
                                string fileName = Path.GetFileNameWithoutExtension(filePath);
                                // Имя выходного файла: имя_исходного_файла_analysis.txt (можно .json, зависит от настроек)
                                string outputFile = Path.Combine(baseOutputDir, $"{fileName}_analysis.txt");

                                await Get_log.LoggerAsync($"[INFO] [MenuSettingsFormation.MenuEditSetting] Обработка файла | index={fileIndex}/{ReadLinesAsync.ListPathInLog.Count}, file='{filePath}'");

                                // Создаём отдельный анализатор для одного файла
                                var appStats = new Statistics();
                                var appPrinter = new LogReportPrinter { ShowInConsole = false, DisableAllLogging = true };
                                var processor = new LogStreamProcessor(appStats, appPrinter);
                                var wordsToSearch = LinkSetting.MyWordList;

                                // Строим поток только из этого файла
                                async IAsyncEnumerable<(string, string, int)> GetSingleFileStream()
                                {
                                    await foreach (var entry in ReadLinesAsync.AsyncReadLine(filePath))
                                        yield return entry;
                                }

                                await processor.AsyncAnalyzer(GetSingleFileStream(), wordsToSearch, printAllLine: false, printFinalReport: false);

                                // Сохраняем отчёт (формат txt с детализацией)
                                await Writer.WriteDetailedReportForSingleFile(appStats, outputFile);

                                await Get_log.LoggerAsync($"[INFO] [MenuSettingsFormation.MenuEditSetting] Отчёт сохранён | file='{outputFile}'");
                            }

                            await Get_log.LoggerAsync("[INFO] [MenuSettingsFormation.MenuEditSetting] Анализ всех файлов завершён");
                            break;
                        }
                    case 7:
                        Console.WriteLine("Exit");
                        Console.Clear();
                        work = false;
                        break;
                }
            }

            await Get_log.LoggerAsync("[DEBUG] [MenuSettingsFormation.MenuEditSetting] Выход из метода | result=успешно");
        }
    }
}