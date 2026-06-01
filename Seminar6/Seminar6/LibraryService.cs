using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Seminar6
{
    // --- Abstractions ---

    public interface ILogger
    {
        void Log(string message);
    }

    public interface IReportExporter
    {
        void Export(string content, string fileName);
    }

    // --- Domain Models ---

    public abstract class LibraryItem
    {
        public string Title { get; }

        protected LibraryItem(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название не может быть пустым");
            Title = title;
        }

        public abstract string GetDisplayInfo();
    }

    public class Book : LibraryItem
    {
        public string Author { get; }
        public int Year { get; }

        public Book(string title, string author, int year) : base(title)
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Автор не может быть пустым");
            if (year < 1000 || year > DateTime.Now.Year)
                throw new ArgumentException("Некорректный год издания");

            Author = author;
            Year = year;
        }

        public override string GetDisplayInfo() => $"Книга: {Title}";
    }

    public class Magazine : LibraryItem
    {
        public int IssueNumber { get; }

        public Magazine(string title, int issueNumber) : base(title)
        {
            if (issueNumber <= 0)
                throw new ArgumentException("Номер выпуска должен быть положительным");

            IssueNumber = issueNumber;
        }

        public override string GetDisplayInfo() => $"Журнал: {Title}";
    }

    // --- Implementations ---

    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        public FileLogger(string filePath) => _filePath = filePath;
        public void Log(string message) => File.AppendAllText(_filePath, $"{DateTime.Now:u}: {message}\n");
    }

    public class FileReportExporter : IReportExporter
    {
        public void Export(string content, string fileName) => File.WriteAllText(fileName, content);
    }

    // --- Refactored Service ---

    public class LibraryService
    {
        private readonly List<LibraryItem> _items = new List<LibraryItem>();
        private readonly ILogger _logger;
        private readonly IReportExporter _exporter;

        public LibraryService(ILogger logger, IReportExporter exporter)
        {
            _logger = logger;
            _exporter = exporter;
        }

        public void AddItem(LibraryItem item)
        {
            _items.Add(item);
            _logger.Log($"Добавлен элемент: {item.GetDisplayInfo()}");
        }

        public void PrintReport()
        {
            Console.WriteLine($"=== Отчёт: {_items.Count} элементов ===");
            foreach (var item in _items)
            {
                Console.WriteLine(item.GetDisplayInfo());
            }

            string reportContent = $"Всего элементов: {_items.Count}\nДата: {DateTime.Now:u}";
            _exporter.Export(reportContent, "report.txt");
        }
    }
}
