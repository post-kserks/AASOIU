/// <summary>
/// Наивный логгер: вывод в консоль (сильная связанность).
/// </summary>
class NaiveConsoleLogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

/// <summary>
/// Наивный сервис: сам создаёт логгер через new.
/// </summary>
class NaiveBookCatalogService
{
    private readonly NaiveConsoleLogger _logger = new();

    public void AddBook(string title, string author) =>
        _logger.Log($"Добавлена книга: «{title}» — {author}");

    public void RemoveBook(string title) =>
        _logger.Log($"Удалена книга: «{title}»");
}
