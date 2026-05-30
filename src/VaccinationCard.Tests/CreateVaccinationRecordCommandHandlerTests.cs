using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Features.VaccinationRecords.Commands.CreateVaccinationRecord;
using VaccinationCard.Domain.Constants;
using VaccinationCard.Domain.Entities;
using VaccinationCard.Infrastructure.Persistence;
using VaccinationCard.Infrastructure.Persistence.Repositories;
using Xunit;

namespace VaccinationCard.Tests;

public class CreateVaccinationRecordCommandHandlerTests
{
    private static CreateVaccinationRecordCommandHandler BuildHandler(ApplicationDbContext context) =>
        new(
            new PersonRepository(context),
            new VaccineRepository(context),
            new VaccinationRecordRepository(context),
            new UnitOfWork(context));

    // Verifica se o registro é salvo no BD se vacina e pessoa existem
    [Fact]
    public async Task Handle_PersonAndVaccineExist_RegistersVaccination()
    {
        using ApplicationDbContext context = TestDbContextFactory.Create();

        var person = new Person { Id = Guid.NewGuid(), Name = "Maria", IdentificationNumber = "111" };
        var vaccine = new Vaccine { Id = Guid.NewGuid(), Name = "COVID-19" };
        context.People.Add(person);
        context.Vaccines.Add(vaccine);
        await context.SaveChangesAsync();

        var command = new CreateVaccinationRecordCommand(
            person.Id, vaccine.Id, VaccineDoses.First, new DateTime(2024, 3, 10));

        Guid recordId = await BuildHandler(context).Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, recordId);

        var saved = await context.VaccinationRecords.FindAsync(recordId);
        Assert.NotNull(saved);
        Assert.Equal(person.Id, saved!.PersonId);
        Assert.Equal(vaccine.Id, saved.VaccineId);
        Assert.Equal(VaccineDoses.First, saved.Dose);
    }

    // Verifica se uma exceção é lançada ao tentar criar um registro de vacinação com uma vacina que não existe
    [Fact]
    public async Task Handle_VaccineDoesNotExist_ThrowsNotFoundException()
    {
        using ApplicationDbContext context = TestDbContextFactory.Create();

        var person = new Person { Id = Guid.NewGuid(), Name = "Maria", IdentificationNumber = "111" };
        context.People.Add(person);
        await context.SaveChangesAsync();

        var command = new CreateVaccinationRecordCommand(
            person.Id, Guid.NewGuid(), VaccineDoses.First, new DateTime(2024, 3, 10));

        await Assert.ThrowsAsync<NotFoundException>(
            () => BuildHandler(context).Handle(command, CancellationToken.None));
    }
}
