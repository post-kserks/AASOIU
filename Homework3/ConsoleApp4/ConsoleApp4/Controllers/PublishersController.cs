using ConsoleApp4.Data;
using ConsoleApp4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp4.Controllers;

/// <summary>
/// Контроллер CRUD-операций справочника издательств.
/// </summary>
public class PublishersController : Controller
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    /// <summary>
    /// Создаёт контроллер справочника издательств.
    /// </summary>
    /// <param name="dbContextFactory">Фабрика контекстов базы данных.</param>
    public PublishersController(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Отображает список всех издательств.
    /// </summary>
    /// <returns>Представление со списком издательств.</returns>
    public IActionResult Index()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var publishers = context.Publishers
            .Include(publisher => publisher.Magazines)
            .OrderBy(publisher => publisher.Name)
            .ToList();

        return View(publishers);
    }

    /// <summary>
    /// Отображает форму добавления издательства.
    /// </summary>
    /// <returns>Представление формы добавления.</returns>
    public IActionResult Create()
    {
        return View(new Publisher());
    }

    /// <summary>
    /// Добавляет новое издательство.
    /// </summary>
    /// <param name="publisher">Данные издательства из формы.</param>
    /// <returns>Переход к списку или форма с ошибками валидации.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Publisher publisher)
    {
        if (!ModelState.IsValid)
        {
            return View(publisher);
        }

        using var context = _dbContextFactory.CreateDbContext();
        if (context.Publishers.Any(existing => existing.Name == publisher.Name))
        {
            ModelState.AddModelError(nameof(Publisher.Name), "Издательство с таким названием уже существует.");
            return View(publisher);
        }

        context.Publishers.Add(publisher);
        context.SaveChanges();
        TempData["StatusMessage"] = "Издательство добавлено.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Отображает форму редактирования издательства.
    /// </summary>
    /// <param name="id">Идентификатор издательства.</param>
    /// <returns>Представление формы редактирования или ответ 404.</returns>
    public IActionResult Edit(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var publisher = context.Publishers.Find(id);
        if (publisher is null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    /// <summary>
    /// Сохраняет изменения издательства.
    /// </summary>
    /// <param name="id">Идентификатор издательства.</param>
    /// <param name="publisher">Данные издательства из формы.</param>
    /// <returns>Переход к списку или форма с ошибками валидации.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Publisher publisher)
    {
        if (id != publisher.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(publisher);
        }

        using var context = _dbContextFactory.CreateDbContext();
        if (context.Publishers.Any(existing => existing.Id != id && existing.Name == publisher.Name))
        {
            ModelState.AddModelError(nameof(Publisher.Name), "Издательство с таким названием уже существует.");
            return View(publisher);
        }

        var existingPublisher = context.Publishers.Find(id);
        if (existingPublisher is null)
        {
            return NotFound();
        }

        existingPublisher.Name = publisher.Name.Trim();
        context.SaveChanges();
        TempData["StatusMessage"] = "Издательство обновлено.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Отображает страницу подтверждения удаления издательства.
    /// </summary>
    /// <param name="id">Идентификатор издательства.</param>
    /// <returns>Представление подтверждения или ответ 404.</returns>
    public IActionResult Delete(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var publisher = context.Publishers
            .Include(item => item.Magazines)
            .FirstOrDefault(item => item.Id == id);
        if (publisher is null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    /// <summary>
    /// Удаляет издательство, если с ним не связаны журналы.
    /// </summary>
    /// <param name="id">Идентификатор издательства.</param>
    /// <returns>Переход к списку или страница подтверждения с предупреждением.</returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var publisher = context.Publishers
            .Include(item => item.Magazines)
            .FirstOrDefault(item => item.Id == id);
        if (publisher is null)
        {
            return NotFound();
        }

        if (publisher.Magazines.Any())
        {
            ModelState.AddModelError(string.Empty, "Нельзя удалить издательство, пока у него есть связанные журналы.");
            return View(publisher);
        }

        context.Publishers.Remove(publisher);
        context.SaveChanges();
        TempData["StatusMessage"] = "Издательство удалено.";

        return RedirectToAction(nameof(Index));
    }
}
