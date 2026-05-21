/// <summary>
/// Сервис каталога книг (исправленное контрольное задание).
/// Зависимости передаются через конструктор, без антипаттернов.
/// </summary>
class BookCatalogService
{
    private readonly ILogger _logger;
    private readonly IBookStorage _storage;

    public BookCatalogService(ILogger logger, IBookStorage storage)
    {
        _logger = logger;
        _storage = storage;
    }

    public void AddBook(string title, string author)
    {
        _storage.Save(title, author);
        _logger.Log($"Добавлена книга: «{title}» — {author}");
    }

    public void RemoveBook(string title) =>
        _logger.Log($"Удалена книга: «{title}»");
}
