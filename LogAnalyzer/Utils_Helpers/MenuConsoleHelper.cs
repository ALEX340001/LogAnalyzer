using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Data;

namespace LogAnalyzer.Utils_Helpers
{
    internal class MenuConsoleHelper
    {
        public static void ShowConfigBlock()
        {
            // Получаем текущие настройки
            var s = LogAnalyzerSettings.Setting;

            // Сохраняем исходный цвет консоли, чтобы вернуть его в конце
            ConsoleColor originalColor = Console.ForegroundColor;

            // 1. Верхняя граница и заголовок
            Console.ForegroundColor = ConsoleColor.Cyan; // Яркий цвет для рамки и заголовка
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.White; // Белый цвет для текста заголовка
            Console.WriteLine("Конфигурация LogAnalyzer");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("───────────────────────────────────────────────────────");

            // 2. Вывод путей (Название — серым, сам путь — зеленым)
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("   Корневая папка:     ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s.DefaultSavePath);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("   Папка с конфигами:  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s.SettingsFolderPath);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("   Текущий файл:       ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Path.GetFileName(s.SettingsFilePath)); // Выведет только "setting.json"

            // 3. Нижняя граница
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════");

            // Возвращаем стандартный цвет консоли
            Console.ForegroundColor = originalColor;
            Console.WriteLine(); // Пустая строка для отступа перед меню
        }
    }
}
