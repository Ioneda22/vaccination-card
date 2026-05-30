using VaccinationCard.Application.Features.Vaccines.Commands.CreateVaccine;
using VaccinationCard.Infrastructure.Persistence;
using VaccinationCard.Infrastructure.Persistence.Repositories;
using Xunit;

namespace VaccinationCard.Tests;

public class CreateVaccineCommandHandlerTests
{   
    // Verifica se uma vacina persiste no banco ao ser criada
    [Fact]
    public async Task Handle_ValidCommand_CreatesVaccineAndSavesToDatabase()
    {
        using ApplicationDbContext context = TestDbContextFactory.Create();
        var repository = new VaccineRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new CreateVaccineCommandHandler(repository, unitOfWork);

        var command = new CreateVaccineCommand("COVID-19 (Pfizer)");

        Guid vaccineId = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, vaccineId);

        var saved = await context.Vaccines.FindAsync(vaccineId);
        Assert.NotNull(saved);
        Assert.Equal("COVID-19 (Pfizer)", saved!.Name);
    }
}
