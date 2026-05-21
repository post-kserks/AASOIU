/// <summary>
/// Хранение книг в оперативной памяти.
/// </summary>
class InMemoryBookStorage : IBookStorage
{
    private readonly ILogger _logger;
    private readonly List<string> _books = new();

    public InMemoryBookStorage(ILogger logger) => _logger = logger;

    public void Save(string title, string author)
    {
        _books.Add($"«{title}» — {author}");
        _logger.Log($" [STORAGE] Сохранено: «{title}»");
    }
}
