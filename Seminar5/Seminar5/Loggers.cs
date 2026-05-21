/// <summary>
/// Логгер: выводит сообщения в консоль.
/// </summary>
class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

/// <summary>
/// Логгер: записывает сообщения в файл.
/// </summary>
class FileLogger : ILogger
{
    private readonly string _filePath;

    public FileLogger(string filePath) => _filePath = filePath;

    public void Log(string message) => File.AppendAllText(_filePath, $"[LOG] {message}\n");
}

/// <summary>
/// Логгер-заглушка: ничего не делает.
/// </summary>
class NullLogger : ILogger
{
    public void Log(string message) { }
}
