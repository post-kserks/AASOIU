using ConsoleApp4.Data;
using ConsoleApp4.Models;
using ConsoleApp4.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp4.Controllers;

/// <summary>
/// Контроллер CRUD-операций основной таблицы журналов.
/// </summary>
public class MagazinesController : Controller
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    /// <summary>
    /// Создаёт контроллер журналов.
    /// </summary>
    /// <param name="dbContextFactory">Фабрика контекстов базы данных.</param>
    public MagazinesController(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Отображает список всех журналов с названиями издательств.
    /// </summary>
    /// <returns>Представление со списком журналов.</returns>
    public IActionResult Index()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var magazines = context.Magazines
            .Include(magazine => magazine.Publisher)
            .OrderBy(magazine => magazine.Name)
            .ToList();

        return View(magazines);
    }

    /// <summary>
    /// Отображает форму добавления журнала.
    /// </summary>
    /// <returns>Представление формы добавления.</returns>
    public IActionResult Create()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return View(new MagazineFormViewModel
        {
            Publishers = BuildPublisherSelectList(context)
        });
    }

    /// <summary>
    /// Добавляет новый журнал.
    /// </summary>
    /// <param name="viewModel">Данные формы журнала.</param>
    /// <returns>Переход к списку или форма с ошибками валидации.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(MagazineFormViewModel viewModel)
    {
        using var context = _dbContextFactory.CreateDbContext();
        ValidateSelectedPublisher(context, viewModel.PublisherId);

        if (!ModelState.IsValid)
        {
            viewModel.Publishers = BuildPublisherSelectList(context, viewModel.PublisherId);
            return View(viewModel);
        }

        context.Magazines.Add(new Magazine
        {
            Name = viewModel.Name.Trim(),
            PublisherId = viewModel.PublisherId,
            CirculationK = viewModel.CirculationK
        });
        context.SaveChanges();
        TempData["StatusMessage"] = "Журнал добавлен.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Отображает форму редактирования журнала.
    /// </summary>
    /// <param name="id">Идентификатор журнала.</param>
    /// <returns>Представление формы редактирования или ответ 404.</returns>
    public IActionResult Edit(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var magazine = context.Magazines.Find(id);
        if (magazine is null)
        {
            return NotFound();
        }

        return View(new MagazineFormViewModel
        {
            Id = magazine.Id,
            Name = magazine.Name,
            PublisherId = magazine.PublisherId,
            CirculationK = magazine.CirculationK,
            Publishers = BuildPublisherSelectList(context, magazine.PublisherId)
        });
    }

    /// <summary>
    /// Сохраняет изменения журнала.
    /// </summary>
    /// <param name="id">Идентификатор журнала.</param>
    /// <param name="viewModel">Данные формы журнала.</param>
    /// <returns>Переход к списку или форма с ошибками валидации.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, MagazineFormViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        using var context = _dbContextFactory.CreateDbContext();
        ValidateSelectedPublisher(context, viewModel.PublisherId);

        if (!ModelState.IsValid)
        {
            viewModel.Publishers = BuildPublisherSelectList(context, viewModel.PublisherId);
            return View(viewModel);
        }

        var magazine = context.Magazines.Find(id);
        if (magazine is null)
        {
            return NotFound();
        }

        magazine.Name = viewModel.Name.Trim();
        magazine.PublisherId = viewModel.PublisherId;
        magazine.CirculationK = viewModel.CirculationK;
        context.SaveChanges();
        TempData["StatusMessage"] = "Журнал обновлён.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Отображает страницу подтверждения удаления журнала.
    /// </summary>
    /// <param name="id">Идентификатор журнала.</param>
    /// <returns>Представление подтверждения или ответ 404.</returns>
    public IActionResult Delete(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var magazine = context.Magazines
            .Include(item => item.Publisher)
            .FirstOrDefault(item => item.Id == id);
        if (magazine is null)
        {
            return NotFound();
        }

        return View(magazine);
    }

    /// <summary>
    /// Удаляет журнал.
    /// </summary>
    /// <param name="id">Идентификатор журнала.</param>
    /// <returns>Переход к списку журналов.</returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var magazine = context.Magazines.Find(id);
        if (magazine is null)
        {
            return NotFound();
        }

        context.Magazines.Remove(magazine);
        context.SaveChanges();
        TempData["StatusMessage"] = "Журнал удалён.";

        return RedirectToAction(nameof(Index));
    }

    private static IReadOnlyList<SelectListItem> BuildPublisherSelectList(AppDbContext context, int? selectedPublisherId = null)
    {
        return context.Publishers
            .OrderBy(publisher => publisher.Name)
            .Select(publisher => new SelectListItem
            {
                Value = publisher.Id.ToString(),
                Text = publisher.Name,
                Selected = selectedPublisherId.HasValue && publisher.Id == selectedPublisherId.Value
            })
            .ToList();
    }

    private void ValidateSelectedPublisher(AppDbContext context, int publisherId)
    {
        if (!context.Publishers.Any(publisher => publisher.Id == publisherId))
        {
            ModelState.AddModelError(nameof(MagazineFormViewModel.PublisherId), "Выберите существующее издательство.");
        }
    }
}
