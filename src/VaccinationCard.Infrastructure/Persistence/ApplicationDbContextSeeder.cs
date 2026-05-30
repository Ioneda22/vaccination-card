using Microsoft.EntityFrameworkCore;
using VaccinationCard.Domain.Constants;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Infrastructure.Persistence;

// Popula o banco InMemory com dados de demonstração no startup.
public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Vaccines.AnyAsync(cancellationToken))
        {
            return;
        }

        var covid = new Vaccine { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name = "COVID-19" };
        var hepatiteB = new Vaccine { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name = "Hepatite B" };
        var febreAmarela = new Vaccine { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name = "Febre Amarela" };
        var tripliceViral = new Vaccine { Id = new Guid("44444444-4444-4444-4444-444444444444"), Name = "Tríplice Viral (SCR)" };
        var influenza = new Vaccine { Id = new Guid("55555555-5555-5555-5555-555555555555"), Name = "Influenza" };

        await context.Vaccines.AddRangeAsync(
            new[] { covid, hepatiteB, febreAmarela, tripliceViral, influenza }, cancellationToken);

        var maria = new Person { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Maria Silva", IdentificationNumber = "12345678900" };
       
        var joao = new Person { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "João Souza", IdentificationNumber = "98765432100" };
       
        var ana = new Person { Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Ana Pereira", IdentificationNumber = "45678912300" };

        await context.People.AddRangeAsync(new[] { maria, joao, ana }, cancellationToken);

        var records = new[]
        {
            // Maria — respeita ordem e cronologia das doses.
            NewRecord(maria.Id, hepatiteB.Id, VaccineDoses.Single, new DateTime(2022, 1, 5)),
            NewRecord(maria.Id, covid.Id, VaccineDoses.First, new DateTime(2023, 2, 10)),
            NewRecord(maria.Id, covid.Id, VaccineDoses.Second, new DateTime(2023, 5, 12)),
            NewRecord(maria.Id, covid.Id, VaccineDoses.Booster, new DateTime(2024, 1, 20)),
            NewRecord(maria.Id, influenza.Id, VaccineDoses.Single, new DateTime(2024, 4, 1)),

            // João — esquema de COVID iniciado.
            NewRecord(joao.Id, covid.Id, VaccineDoses.First, new DateTime(2023, 3, 1)),
            NewRecord(joao.Id, febreAmarela.Id, VaccineDoses.Single, new DateTime(2021, 7, 15)),
        };

        await context.VaccinationRecords.AddRangeAsync(records, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static VaccinationRecord NewRecord(Guid personId, Guid vaccineId, string dose, DateTime applicationDate) =>
        new()
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            VaccineId = vaccineId,
            Dose = dose,
            ApplicationDate = applicationDate
        };
}
