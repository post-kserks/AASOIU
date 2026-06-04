namespace ConsoleApp4.ViewModels;

/// <summary>
/// Модель страницы отчётов по журналам и издательствам.
/// </summary>
public class ReportsViewModel
{
    /// <summary>
    /// Полный список журналов с издательствами.
    /// </summary>
    public IReadOnlyList<MagazineReportRow> Magazines { get; init; } = Array.Empty<MagazineReportRow>();

    /// <summary>
    /// Количество журналов по издательствам.
    /// </summary>
    public IReadOnlyList<PublisherCountReportRow> CountsByPublisher { get; init; } = Array.Empty<PublisherCountReportRow>();

    /// <summary>
    /// Средний тираж по издательствам.
    /// </summary>
    public IReadOnlyList<PublisherAverageReportRow> AverageCirculationByPublisher { get; init; } = Array.Empty<PublisherAverageReportRow>();
}

/// <summary>
/// Строка отчёта полного списка журналов.
/// </summary>
public class MagazineReportRow
{
    /// <summary>
    /// Название журнала.
    /// </summary>
    public string MagazineName { get; init; } = string.Empty;

    /// <summary>
    /// Название издательства.
    /// </summary>
    public string PublisherName { get; init; } = string.Empty;

    /// <summary>
    /// Тираж журнала в тысячах экземпляров.
    /// </summary>
    public int CirculationK { get; init; }
}

/// <summary>
/// Строка отчёта количества журналов по издательству.
/// </summary>
public class PublisherCountReportRow
{
    /// <summary>
    /// Название издательства.
    /// </summary>
    public string PublisherName { get; init; } = string.Empty;

    /// <summary>
    /// Количество журналов издательства.
    /// </summary>
    public int MagazineCount { get; init; }
}

/// <summary>
/// Строка отчёта среднего тиража по издательству.
/// </summary>
public class PublisherAverageReportRow
{
    /// <summary>
    /// Название издательства.
    /// </summary>
    public string PublisherName { get; init; } = string.Empty;

    /// <summary>
    /// Средний тираж журналов в тысячах экземпляров.
    /// </summary>
    public double AverageCirculationK { get; init; }
}
