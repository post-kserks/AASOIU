using System;

namespace Seminar6
{
    class Program
    {
        static void Main(string[] args)
        {
            // Точка сборки (Composition Root)
            ILogger logger = new FileLogger("library.log");
            IReportExporter exporter = new FileReportExporter();
            LibraryService library = new LibraryService(logger, exporter);

            try
            {
                library.AddItem(new Book("Война и мир", "Лев Толстой", 1869));
                library.AddItem(new Magazine("Наука и жизнь", 5));
                
                library.PrintReport();
                
                Console.WriteLine("Задание выполнено успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}