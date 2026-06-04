using ConsoleApp4.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp4.Data;

/// <summary>
/// Контекст базы данных приложения для издательств и журналов.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Создаёт контекст базы данных с заданными параметрами EF Core.
    /// </summary>
    /// <param name="options">Параметры подключения и провайдера базы данных.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Справочник издательств.
    /// </summary>
    public DbSet<Publisher> Publishers => Set<Publisher>();

    /// <summary>
    /// Основная таблица журналов.
    /// </summary>
    public DbSet<Magazine> Magazines => Set<Magazine>();

    /// <summary>
    /// Настраивает схему модели и ограничения связи один-ко-многим.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели Entity Framework Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("publishers");
            entity.HasKey(publisher => publisher.Id);
            entity.Property(publisher => publisher.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity.HasIndex(publisher => publisher.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Magazine>(entity =>
        {
            entity.ToTable("magazines");
            entity.HasKey(magazine => magazine.Id);
            entity.Property(magazine => magazine.Name)
                .HasMaxLength(120)
                .IsRequired();
            entity.Property(magazine => magazine.CirculationK)
                .IsRequired();
            entity.HasOne(magazine => magazine.Publisher)
                .WithMany(publisher => publisher.Magazines)
                .HasForeignKey(magazine => magazine.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
