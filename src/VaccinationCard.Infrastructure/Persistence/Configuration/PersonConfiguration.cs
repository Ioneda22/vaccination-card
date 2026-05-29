using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Infrastructure.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(person => person.Id);

        builder.Property(person => person.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(person => person.IdentificationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(person => person.IdentificationNumber)
            .IsUnique();

        builder.HasMany(person => person.VaccinationRecords) 
            .WithOne(record => record.Person)
            .HasForeignKey(record => record.PersonId)
            .OnDelete(DeleteBehavior.Cascade); // Ao deletar uma pessoa, deleta-se todas os registros de vacina dela
                                               
    }
}
