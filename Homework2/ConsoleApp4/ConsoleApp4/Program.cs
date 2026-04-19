using System.Text;
using ConsoleApp4;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
string dbPath = Path.Combine(AppContext.BaseDirectory, "magazines.db");
string pubCsv = Path.Combine(AppContext.BaseDirectory, "pub.csv");
string magCsv = Path.Combine(AppContext.BaseDirectory, "mag.csv");
var db = new DatabaseManager(dbPath);
db.InitializeDatabase(pubCsv, magCsv);

Console.WriteLine();
string choice;
do
{
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║     ЖУРНАЛЫ И ИЗДАТЕЛЬСТВА           ║");
    Console.WriteLine("╠══════════════════════════════════════╣");
    Console.WriteLine("║  1 — Показать все издательства       ║");
    Console.WriteLine("║  2 — Показать все журналы            ║");
    Console.WriteLine("║  3 — Добавить журнал                 ║");
    Console.WriteLine("║  4 — Редактировать журнал            ║");
    Console.WriteLine("║  5 — Удалить журнал                  ║");
    Console.WriteLine("║  6 — Отчёты                          ║");
    Console.WriteLine("║  0 — Выход                           ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.Write("Ваш выбор: ");

    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();

    switch (choice)
    {
        case "1": ShowPublishers(db); break;
        case "2": ShowMagazines(db); break;
        case "3": AddMagazine(db); break;
        case "4": EditMagazine(db); break;
        case "5": DeleteMagazine(db); break;
        case "6": ReportsMenu(db); break;
        case "0": Console.WriteLine("До свидания!"); break;
        default: Console.WriteLine("Неверный пункт меню."); break;
    }

    Console.WriteLine();
}
while (choice != "0");

static void ShowPublishers(DatabaseManager db)
{
    Console.WriteLine("--- Все издательства ---");
    var publishers = db.GetAllPublishers();
    foreach (var pub in publishers)
        Console.WriteLine("  " + pub);
    Console.WriteLine($"Итого: {publishers.Count}");
}

static void ShowMagazines(DatabaseManager db)
{
    Console.WriteLine("--- Все журналы ---");
    var magazines = db.GetAllMagazines();
    foreach (var mag in magazines)
        Console.WriteLine("  " + mag);
    Console.WriteLine($"Итого: {magazines.Count}");
}

static void AddMagazine(DatabaseManager db)
{
    Console.WriteLine("--- Добавление журнала ---");
    Console.WriteLine("Доступные издательства:");
    var publishers = db.GetAllPublishers();
    foreach (var pub in publishers)
        Console.WriteLine("  " + pub);

    Console.Write("ID издательства: ");
    if (!int.TryParse(Console.ReadLine(), out int pubId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    Console.Write("Название журнала: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (name.Length == 0)
    {
        Console.WriteLine("Ошибка: название не может быть пустым.");
        return;
    }

    Console.Write("Тираж (тыс. экз.): ");
    if (!int.TryParse(Console.ReadLine(), out int circ))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    try
    {
        var mag = new Magazine(0, pubId, name, circ);
        db.AddMagazine(mag);
        Console.WriteLine("Журнал добавлен.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void EditMagazine(DatabaseManager db)
{
    Console.WriteLine("--- Редактирование журнала ---");
    Console.Write("Введите ID журнала: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var mag = db.GetMagazineById(id);
    if (mag == null)
    {
        Console.WriteLine($"Журнал с ID={id} не найден.");
        return;
    }

    Console.WriteLine($"Текущие данные: {mag}");
    Console.WriteLine("(нажмите Enter, чтобы оставить значение без изменений)");
    Console.Write($"Название [{mag.Name}]: ");
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0)
        mag.Name = input;
    Console.Write($"ID издательства [{mag.PublisherId}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newPubId))
        mag.PublisherId = newPubId;
    Console.Write($"Тираж (тыс. экз.) [{mag.CirculationK}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newCirc))
    {
        try
        {
            mag.CirculationK = newCirc;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return;
        }
    }

    db.UpdateMagazine(mag);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteMagazine(DatabaseManager db)
{
    Console.WriteLine("--- Удаление журнала ---");
    Console.Write("Введите ID журнала: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var mag = db.GetMagazineById(id);
    if (mag == null)
    {
        Console.WriteLine($"Журнал с ID={id} не найден.");
        return;
    }

    Console.Write($"Удалить «{mag.Name}»? (да/нет): ");
    string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteMagazine(id);
        Console.WriteLine("Журнал удалён.");
    }
    else
    {
        Console.WriteLine("Удаление отменено.");
    }
}

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine("  1 — Полный список журналов с издательствами");
        Console.WriteLine("  2 — Количество журналов по издательствам");
        Console.WriteLine("  3 — Средний тираж по издательствам");
        Console.WriteLine("  0 — Назад");
        Console.Write("Ваш выбор: ");

        choice = Console.ReadLine()?.Trim() ?? "";

        switch (choice)
        {
            case "1": Report1(db); break;
            case "2": Report2(db); break;
            case "3": Report3(db); break;
            case "0": break;
            default: Console.WriteLine("Неверный пункт меню."); break;
        }

        Console.WriteLine();
    }
    while (choice != "0");
}

static void Report1(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT m.mag_name, p.pub_name, m.mag_circulation_k
                 FROM mag m
                 JOIN pub p ON m.pub_id = p.pub_id
                 ORDER BY m.mag_name")
        .Title("Полный список журналов")
        .Header("Журнал", "Издательство", "Тираж (тыс.)")
        .ColumnWidths(5, 22, 18, 15)
        .Numbered()
        .Print();
}

static void Report2(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT p.pub_name, COUNT(*) AS cnt
                 FROM mag m
                 JOIN pub p ON m.pub_id = p.pub_id
                 GROUP BY p.pub_name
                 ORDER BY p.pub_name")
        .Title("Количество журналов по издательствам")
        .Header("Издательство", "Кол-во журналов")
        .ColumnWidths(5, 22, 18)
        .Numbered()
        .Print();
}

static void Report3(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT p.pub_name, ROUND(AVG(m.mag_circulation_k), 1) AS avg_circ
                 FROM mag m
                 JOIN pub p ON m.pub_id = p.pub_id
                 GROUP BY p.pub_name
                 ORDER BY avg_circ DESC")
        .Title("Средний тираж по издательствам")
        .Header("Издательство", "Средний тираж (тыс.)")
        .ColumnWidths(22, 22)
        .Print();
}
