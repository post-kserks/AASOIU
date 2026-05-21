/// <summary>
/// Внедрение зависимости через конструктор.
/// </summary>
class BookCatalogService_DI_Constructor
{
    private readonly ILogger _logger;

    public BookCatalogService_DI_Constructor(ILogger logger) => _logger = logger;

    public void AddBook(string title, string author) =>
        _logger.Log($"Добавлена книга: «{title}» — {author}");

    public void RemoveBook(string title) =>
        _logger.Log($"Удалена книга: «{title}»");
}

/// <summary>
/// Внедрение зависимости через свойство (необязательный логгер).
/// </summary>
class BookCatalogService_DI_Property
{
    public ILogger Logger { get; set; } = new NullLogger();

    public void AddBook(string title, string author) =>
        Logger.Log($"Добавлена книга: «{title}» — {author}");

    public void RemoveBook(string title) =>
        Logger.Log($"Удалена книга: «{title}»");
}

/// <summary>
/// Внедрение зависимости через параметр метода.
/// </summary>
class BookCatalogService_DI_Method
{
    public void AddBook(string title, string author, ILogger logger) =>
        logger.Log($"Добавлена книга: «{title}» — {author}");
}
