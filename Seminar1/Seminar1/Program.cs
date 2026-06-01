using System;
using System.Globalization;
using System.Linq; // Используем LINQ для удобной работы с коллекциями и вычисления статистики

namespace Seminar1;

/// <summary>
/// Программа для анализа квалификационных заездов.
/// Оптимизированная версия (Этап 6).
/// </summary>
class Program
{
    // Используем записи (records) для хранения связанных данных. 
    // Это современный способ (C# 9.0+) группировать данные, заменяющий параллельные массивы.
    public record TeamResult(string TeamName, double AvgSpeed);

    static void Main(string[] args)
    {
        Console.WriteLine("=== АНАЛИЗ КВАЛИФИКАЦИИ ГРАН-ПРИ (ОПТИМИЗИРОВАНО) ===");
        Console.WriteLine();

        // Современный способ получения ввода с проверкой
        int n = GetValidInt("Введите количество участников: ");
        Console.WriteLine();

        // Вместо двух массивов используем один массив объектов (records).
        // Это избавляет от необходимости синхронизировать перестановки в разных массивах.
        TeamResult[] results = new TeamResult[n];

        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"Участник #{i + 1}");
            Console.Write("Команда: ");
            string team = Console.ReadLine() ?? "Unknown";
            double speed = GetValidDouble("Средняя скорость (км/ч): ");
            results[i] = new TeamResult(team, speed);
            Console.WriteLine();
        }

        // Вычисление статистики с использованием LINQ (более читаемо и современно)
        if (results.Any())
        {
            Console.WriteLine("--- СТАТИСТИКА КВАЛИФИКАЦИИ ---");
            double average = results.Average(r => r.AvgSpeed);
            var fastest = results.OrderByDescending(r => r.AvgSpeed).First();
            var slowest = results.OrderBy(r => r.AvgSpeed).First();

            Console.WriteLine($"Средняя скорость: {average:F2} км/ч");
            Console.WriteLine($"Лидер: {fastest.TeamName} ({fastest.AvgSpeed:F2} км/ч)");
            Console.WriteLine($"Самый медленный: {slowest.TeamName} ({slowest.AvgSpeed:F2} км/ч)");
            Console.WriteLine($"Разница темпа: {fastest.AvgSpeed - slowest.AvgSpeed:F2} км/ч");
        }
        Console.WriteLine();

        // Вывод исходного порядка (Интерполяция строк с форматированием)
        Console.WriteLine("--- ИСХОДНЫЙ ПОРЯДОК ---");
        PrintResultsTable(results);
        Console.WriteLine();

        // Сортировка (используем встроенный метод OrderByDescending для краткости, 
        // но сохраняем структуру примера, если нужно показать свой алгоритм)
        // В этой версии мы используем более эффективный алгоритм, встроенный в .NET,
        // который обычно является Timsort или Introsort.
        var sortedResults = results.OrderByDescending(r => r.AvgSpeed).ToArray();

        Console.WriteLine("--- ИТОГОВЫЙ ПРОТОКОЛ КВАЛИФИКАЦИИ ---");
        PrintResultsTable(sortedResults, showPosition: true);
        Console.WriteLine();

        // Фильтрация (также через LINQ)
        double threshold = GetValidDouble("Введите минимальную скорость для отбора (км/ч): ");
        var filtered = sortedResults.Where(r => r.AvgSpeed >= threshold).ToList();

        Console.WriteLine();
        Console.WriteLine($"Команды со скоростью >= {threshold:F2} км/ч:");
        foreach (var r in filtered)
        {
            Console.WriteLine($"- {r.TeamName} ({r.AvgSpeed:F2} км/ч)");
        }
        Console.WriteLine($"\nОтобрано команд: {filtered.Count}");

        Console.WriteLine();
        Console.Write("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    /// <summary>
    /// Универсальный метод для вывода таблицы.
    /// </summary>
    static void PrintResultsTable(TeamResult[] data, bool showPosition = false)
    {
        string header = showPosition 
            ? "| Поз. | Команда              | Скорость |" 
            : "| Команда              | Скорость (км/ч) |";
        
        Console.WriteLine(new string('-', header.Length));
        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));

        for (int i = 0; i < data.Length; i++)
        {
            if (showPosition)
                Console.WriteLine($"| {i + 1,4} | {data[i].TeamName,-20} | {data[i].AvgSpeed,8:F2} |");
            else
                Console.WriteLine($"| {data[i].TeamName,-20} | {data[i].AvgSpeed,15:F2} |");
        }
        Console.WriteLine(new string('-', header.Length));
    }

    // Вспомогательные методы для безопасного ввода
    static int GetValidInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int val) && val > 0) return val;
            Console.WriteLine("Ошибка: введите положительное целое число.");
        }
    }

    static double GetValidDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";
            // Обработка разных разделителей (точка/запятая) для надежности
            if (double.TryParse(input.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double val)) return val;
            Console.WriteLine("Ошибка: введите числовое значение.");
        }
    }
}
