using ConsoleApp4.Data;
using ConsoleApp4.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp4.Controllers;

/// <summary>
/// Контроллер отчётов по журналам и издательствам.
/// </summary>
public class ReportsController : Controller
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    /// <summary>
    /// Создаёт контроллер отчётов.
    /// </summary>
    /// <param name="dbContextFactory">Фабрика контекстов базы данных.</param>
    public ReportsController(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Формирует и отображает три раздела отчёта средствами LINQ.
    /// </summary>
    /// <returns>Представление отчёта.</returns>
    public IActionResult Index()
    {
        using var context = _dbContextFactory.CreateDbContext();

        var magazines = context.Magazines
            .Include(magazine => magazine.Publisher)
            .OrderBy(magazine => magazine.Name)
            .Select(magazine => new MagazineReportRow
            {
                MagazineName = magazine.Name,
                PublisherName = magazine.Publisher!.Name,
                CirculationK = magazine.CirculationK
            })
            .ToList();

        var countsByPublisher = context.Magazines
            .GroupBy(magazine => magazine.Publisher!.Name)
            .Select(group => new PublisherCountReportRow
            {
                PublisherName = group.Key,
                MagazineCount = group.Count()
            })
            .OrderBy(row => row.PublisherName)
            .ToList();

        var averageCirculationByPublisher = context.Magazines
            .GroupBy(magazine => magazine.Publisher!.Name)
            .Select(group => new PublisherAverageReportRow
            {
                PublisherName = group.Key,
                AverageCirculationK = group.Average(magazine => magazine.CirculationK)
            })
            .OrderByDescending(row => row.AverageCirculationK)
            .ToList();

        var viewModel = new ReportsViewModel
        {
            Magazines = magazines,
            CountsByPublisher = countsByPublisher,
            AverageCirculationByPublisher = averageCirculationByPublisher
        };

        return View(viewModel);
    }
}
