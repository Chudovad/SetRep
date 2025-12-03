using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SetRep.Infrastructure.Data;

/// <summary>
/// Фабрика для создания DbContext во время разработки (для миграций)
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Используем строку подключения по умолчанию для разработки
        // В продакшене это будет переопределено через конфигурацию
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SetRepDb;Username=postgres;Password=localdb");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}