using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Features.People.Commands.CreatePerson;
using VaccinationCard.Domain.Entities;
using VaccinationCard.Infrastructure.Persistence;
using VaccinationCard.Infrastructure.Persistence.Repositories;
using Xunit;

namespace VaccinationCard.Tests;

public class CreatePersonCommandHandlerTests
{   

    // Verifica se a pessoa persiste no banco ao ser criada 
    [Fact]
    public async Task Handle_ValidCommand_CreatesPersonAndSavesToDatabase()
    {
        using ApplicationDbContext context = TestDbContextFactory.Create();
        var handler = new CreatePersonCommandHandler(
            new PersonRepository(context),
            new UnitOfWork(context));

        var command = new CreatePersonCommand("Maria Silva", "12345678900");

        
        Guid personId = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, personId);

        var saved = await context.People.FindAsync(personId);
        Assert.NotNull(saved);
        Assert.Equal("Maria Silva", saved!.Name);
        Assert.Equal("12345678900", saved.IdentificationNumber);
    }

    // Verifica se uma exceção é lançada ao tentar criar uma pessoa que ja existe no banco
    [Fact]
    public async Task Handle_IdentificationAlreadyExists_ThrowsConflictException()
    {
        using ApplicationDbContext context = TestDbContextFactory.Create();
        context.People.Add(new Person
        {
            Id = Guid.NewGuid(),
            Name = "Pessoa Existente",
            IdentificationNumber = "12345678900"
        });
        await context.SaveChangesAsync();

        var handler = new CreatePersonCommandHandler(
            new PersonRepository(context),
            new UnitOfWork(context));

        var command = new CreatePersonCommand("Outra Pessoa", "12345678900");

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
