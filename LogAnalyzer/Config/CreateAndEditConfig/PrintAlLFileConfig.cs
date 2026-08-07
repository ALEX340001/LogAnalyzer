using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Data;

namespace LogAnalyzer.Config.CreateAndEditConfig
{
        internal class PrintAlLFileConfig
        {
            // Метод выводит список и ВОЗВРАЩАЕТ путь, который выберет пользователь
            public static string SelectConfigFromHistory(LogAnalyzerSettings LinkSetting)
            {
                var items = LinkSetting.PathSaveFloaderConfig;

                // 1. Проверяем, есть ли вообще что выбирать
                if (items == null || !items.Any())
                {
                    Console.WriteLine("History of paths is empty. Using default path.");
                    return LinkSetting.DefaultSavePath;
                }

                // 2. Выводим элементы и сразу показываем их реальные номера (начиная с 1)
                Console.WriteLine("\n--- Available configurations ---");
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine($"[{i + 1}] {items[i]}");
                }

                // 3. Цикл для безопасного выбора номера
                while (true)
                {
                    Console.Write($"\nSelect config number (1-{items.Count}): ");
                    string input = Console.ReadLine();

                    // Проверяем: ввел ли пользователь число, и входит ли оно в диапазон списка
                    if (int.TryParse(input, out int userChoice) && userChoice >= 1 && userChoice <= items.Count)
                    {
                        // Вычитаем 1, так как в C# индексы списка начинаются с 0, а для пользователя мы выводили с 1
                        int targetIndex = userChoice - 1;

                        string selectedPath = items[targetIndex];
                        Console.WriteLine($"You selected: {selectedPath}");

                        return selectedPath;
                    }

                    // Если пользователь ошибся (ввел буквы или число вне диапазона)
                    Console.WriteLine($"Invalid input. Please enter a number between 1 and {items.Count}.");
                }
            }
        }
    }

