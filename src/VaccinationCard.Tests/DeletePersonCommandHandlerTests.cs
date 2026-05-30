using VaccinationCard.Application.Features.People.Commands.DeletePerson;
using VaccinationCard.Domain.Constants;
using VaccinationCard.Domain.Entities;
using VaccinationCard.Infrastructure.Persistence;
using VaccinationCard.Infrastructure.Persistence.Repositories;
using Xunit;

namespace VaccinationCard.Tests;

public class DeletePersonCommandHandlerTests
{   

    // Verifica se os registros de vacinação de uma pessoa são deletados em cascata ao deletar essa pessoa
    [Fact]
    public async Task Handle_ExistingPerson_RemovesPersonAndCascadesRecords()
    {
        using ApplicationDbContext context = TestDbContextFactory.Create();

        var person = new Person { Id = Guid.NewGuid(), Name = "Maria", IdentificationNumber = "111" };
        var vaccine = new Vaccine { Id = Guid.NewGuid(), Name = "COVID-19" };
        context.People.Add(person);
        context.Vaccines.Add(vaccine);
        context.VaccinationRecords.AddRange(
            new VaccinationRecord
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                VaccineId = vaccine.Id,
                Dose = VaccineDoses.First,
                ApplicationDate = new DateTime(2024, 1, 10)
            },
            new VaccinationRecord
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                VaccineId = vaccine.Id,
                Dose = VaccineDoses.Second,
                ApplicationDate = new DateTime(2024, 2, 10)
            });
        await context.SaveChangesAsync();

        Assert.Equal(2, context.VaccinationRecords.Count());

        var handler = new DeletePersonCommandHandler(
            new PersonRepository(context),
            new UnitOfWork(context));

        await handler.Handle(new DeletePersonCommand(person.Id), CancellationToken.None);

        Assert.Null(await context.People.FindAsync(person.Id));
        Assert.Empty(context.VaccinationRecords);

        Assert.NotNull(await context.Vaccines.FindAsync(vaccine.Id));
    }
}
