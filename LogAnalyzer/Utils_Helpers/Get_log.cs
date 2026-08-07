namespace MyNotes.Utils
{
    internal class Get_log
    {
        // Используем SemaphoreSlim для предотвращения одновременного доступа к файлу
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public static async Task LoggerAsync(string message)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string logFolderPath = Path.Combine(documentsPath, "MyLogger_Program_log");

            // Directory.CreateDirectory сам проверяет существование папки, 
            // поэтому if (!Directory.Exists) можно опустить.
            Directory.CreateDirectory(logFolderPath);

            string filePath = Path.Combine(logFolderPath, $"{DateTime.Now:yyyy-MM-dd}_log.txt");
            string logEntry = $"time: {DateTime.Now:HH:mm:ss} message: {message}{Environment.NewLine}";

            // Ждем своей очереди на запись
            await _semaphore.WaitAsync();
            try
            {
                await File.AppendAllTextAsync(filePath, logEntry);
            }
            catch (Exception ex)
            {
                // Пишем реальную ошибку, чтобы знать, что пошло не так
                Console.WriteLine($"Ошибка логирования: {ex.Message}");
            }
            finally
            {
                // Обязательно освобождаем семафор
                _semaphore.Release();
            }
        }
    }
}
