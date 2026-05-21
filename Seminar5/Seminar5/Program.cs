using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== Наивная реализация ===");
NaiveBookCatalogService naive = new();
naive.AddBook("Евгений Онегин", "Пушкин");
naive.RemoveBook("Евгений Онегин");

Console.WriteLine("\n=== Внедрение через конструктор ===");
BookCatalogService_DI_Constructor s1 = new(new ConsoleLogger());
s1.AddBook("Евгений Онегин", "Пушкин");

BookCatalogService_DI_Constructor s2 = new(new FileLogger("log.txt"));
s2.AddBook("Сборник стихотворений", "Пушкин");

Console.WriteLine("\n=== Внедрение через свойство ===");
BookCatalogService_DI_Property s3 = new();
s3.AddBook("Евгений Онегин", "Пушкин");
s3.Logger = new ConsoleLogger();
s3.AddBook("Сборник стихотворений", "Пушкин");

Console.WriteLine("\n=== Внедрение через параметр метода ===");
BookCatalogService_DI_Method s4 = new();
s4.AddBook("Евгений Онегин", "Пушкин", new ConsoleLogger());
s4.AddBook("Сборник стихотворений", "Пушкин", new FileLogger("audit.log"));

Console.WriteLine("\n=== Точка сборки (Pure DI) ===");
ILogger logger = new ConsoleLogger();
IBookStorage storage = new InMemoryBookStorage(logger);
BookCatalogService s5 = new(logger, storage);
s5.AddBook("Евгений Онегин", "Пушкин");
s5.AddBook("Сборник стихотворений", "Пушкин");

Console.WriteLine("\n=== Точка сборки с DI-контейнером ===");
ServiceCollection services = new();
services.AddSingleton<ILogger, ConsoleLogger>();
services.AddSingleton<IBookStorage, InMemoryBookStorage>();
services.AddTransient<BookCatalogService>();

ServiceProvider provider = services.BuildServiceProvider(
    new ServiceProviderOptions { ValidateOnBuild = true });

BookCatalogService s7 = provider.GetRequiredService<BookCatalogService>();
s7.AddBook("Евгений Онегин", "Пушкин");
s7.RemoveBook("Евгений Онегин");

Console.WriteLine("\n=== Контрольное задание (исправленный BookCatalogService) ===");
Console.WriteLine("Устранены антипаттерны:");
Console.WriteLine("  1. Control Freak — убран new в конструкторе без параметров");
Console.WriteLine("  2. Bastard Injection — оставлен один конструктор с зависимостями");
Console.WriteLine("  3. Service Locator — убран AppServices.Get<ILogger>()");
Console.WriteLine("  4. Ambient Context — убран CurrentLogger.Instance");

ILogger ctrlLogger = new ConsoleLogger();
BookCatalogService ctrl = new(ctrlLogger, new InMemoryBookStorage(ctrlLogger));
ctrl.AddBook("Евгений Онегин", "Пушкин");
ctrl.RemoveBook("Евгений Онегин");
