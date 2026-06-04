using ConsoleApp4.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp4.Data;

internal static class DatabaseInitializer
{
    public static void EnsureCreatedAndSeed(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        using var context = dbContextFactory.CreateDbContext();
        context.Database.EnsureCreated();

        if (context.Publishers.Any() || context.Magazines.Any())
        {
            return;
        }

        var publishers = new[]
        {
            new Publisher { Id = 1, Name = "Эксмо" },
            new Publisher { Id = 2, Name = "АСТ" },
            new Publisher { Id = 3, Name = "Просвещение" },
            new Publisher { Id = 4, Name = "Дрофа" }
        };

        var magazines = new[]
        {
            new Magazine { Id = 1, PublisherId = 1, Name = "Наука и жизнь", CirculationK = 40 },
            new Magazine { Id = 2, PublisherId = 1, Name = "Вокруг света", CirculationK = 55 },
            new Magazine { Id = 3, PublisherId = 1, Name = "Техника молодёжи", CirculationK = 30 },
            new Magazine { Id = 4, PublisherId = 2, Name = "Огонёк", CirculationK = 25 },
            new Magazine { Id = 5, PublisherId = 2, Name = "Юность", CirculationK = 20 },
            new Magazine { Id = 6, PublisherId = 2, Name = "Новый мир", CirculationK = 15 },
            new Magazine { Id = 7, PublisherId = 3, Name = "Мурзилка", CirculationK = 60 },
            new Magazine { Id = 8, PublisherId = 3, Name = "Весёлые картинки", CirculationK = 45 },
            new Magazine { Id = 9, PublisherId = 3, Name = "Квант", CirculationK = 10 },
            new Magazine { Id = 10, PublisherId = 4, Name = "Здоровье", CirculationK = 35 },
            new Magazine { Id = 11, PublisherId = 4, Name = "Крестьянка", CirculationK = 28 },
            new Magazine { Id = 12, PublisherId = 4, Name = "Работница", CirculationK = 22 }
        };

        context.Publishers.AddRange(publishers);
        context.Magazines.AddRange(magazines);
        context.SaveChanges();
    }
}
