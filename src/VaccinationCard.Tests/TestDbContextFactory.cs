using Microsoft.EntityFrameworkCore;
using VaccinationCard.Infrastructure.Persistence;

namespace VaccinationCard.Tests;

// Cria um ApplicationDbContext usando o provedor InMemory, com um nome de banco único por teste
internal static class TestDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        return new ApplicationDbContext(options);
    }
}
