using CommandLine;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.ChangingFilePath;
using LogAnalyzer.Core.MenuSettingsFormation.IO_SettingValues.LoadAndSave;
using LogAnalyzer.Core.NoteManager;
using LogAnalyzer.Core.Services;
using LogAnalyzer.Data;
using LogAnalyzer.Utils_Helpers;
using System;
using System.Text;
using System.Threading.Tasks;
using static LogAnalyzer.Data.Initializer;

namespace LogAnalyzer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Гарантируем инфраструктуру (папки, дефолтный конфиг)
            await StartupInitializer.EnsureAppInfrastructureAsync();

            // Загружаем глобальные настройки по умолчанию
            var defaultSettings = await LoadAndSave.LoadAndSaveAsync(LogAnalyzerSettings.Setting.SettingsFilePath);
            LogAnalyzerSettings.Setting = defaultSettings;

            if (args.Length == 0)
            {
                // Интерактивное меню
                await MenuSettingsFormation.MenuEditSetting();
                return;
            }

            // Парсим аргументы
            await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsedAsync(async opts =>
                {
                    // Определяем активную конфигурацию (слияние -c, локальный, глобальный)
                    var activeSettings = await ConfigurationResolver.ResolveAsync(opts);
                    // Синхронизируем ListPathInLog с ChangingFilePath
                    ChangingFilePath.ListPathInLog = !string.IsNullOrEmpty(opts.InputFilePath)
                        ? new List<string> { opts.InputFilePath }
                        : activeSettings.LogPaths ?? new List<string>();

                    // Запускаем оркестратор
                    var orchestrator = new AnalysisOrchestrator(activeSettings, opts);
                    await orchestrator.RunAsync();
                });
        }
    }
}