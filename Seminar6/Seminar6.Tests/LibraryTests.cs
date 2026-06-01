using Xunit;
using Seminar6;
using System;
using System.Collections.Generic;

namespace Seminar6.Tests
{
    public class LibraryTests
    {
        // Stubs for dependencies
        private class TestLogger : ILogger
        {
            public List<string> Logs { get; } = new List<string>();
            public void Log(string message) => Logs.Add(message);
        }

        private class TestExporter : IReportExporter
        {
            public string ExportedContent { get; private set; }
            public void Export(string content, string fileName) => ExportedContent = content;
        }

        [Fact]
        public void Book_Constructor_ThrowsOnInvalidData()
        {
            Assert.Throws<ArgumentException>(() => new Book("", "Author", 2000));
            Assert.Throws<ArgumentException>(() => new Book("Title", "", 2000));
            Assert.Throws<ArgumentException>(() => new Book("Title", "Author", 900));
        }

        [Fact]
        public void Magazine_Constructor_ThrowsOnInvalidData()
        {
            Assert.Throws<ArgumentException>(() => new Magazine("", 1));
            Assert.Throws<ArgumentException>(() => new Magazine("Title", 0));
        }

        [Fact]
        public void LibraryService_AddItem_LogsCorrectly()
        {
            // Arrange
            var logger = new TestLogger();
            var exporter = new TestExporter();
            var service = new LibraryService(logger, exporter);
            var book = new Book("Test Book", "Test Author", 2020);

            // Act
            service.AddItem(book);

            // Assert
            Assert.Single(logger.Logs);
            Assert.Contains("Test Book", logger.Logs[0]);
        }

        [Fact]
        public void LibraryService_PrintReport_ExportsCorrectly()
        {
            // Arrange
            var logger = new TestLogger();
            var exporter = new TestExporter();
            var service = new LibraryService(logger, exporter);
            service.AddItem(new Book("Book 1", "Author 1", 2021));
            service.AddItem(new Magazine("Mag 1", 10));

            // Act
            service.PrintReport();

            // Assert
            Assert.NotNull(exporter.ExportedContent);
            Assert.Contains("Всего элементов: 2", exporter.ExportedContent);
        }
    }
}
