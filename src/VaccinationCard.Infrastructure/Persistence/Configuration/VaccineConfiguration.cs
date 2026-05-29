using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Infrastructure.Persistence.Configurations;

public class VaccineConfiguration : IEntityTypeConfiguration<Vaccine>
{
    public void Configure(EntityTypeBuilder<Vaccine> builder)
    {
        builder.HasKey(vaccine => vaccine.Id);

        builder.Property(vaccine => vaccine.Name)
            .IsRequired()
            .HasMaxLength(250);
        
        // Uma vacina só pode ser deletada se não houver nenhum registro com ela
        builder.HasMany(vaccine => vaccine.VaccinationRecords)
            .WithOne(record => record.Vaccine)
            .HasForeignKey(record => record.VaccineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
