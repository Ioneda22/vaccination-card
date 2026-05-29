using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Infrastructure.Persistence.Configurations;

public class VaccinationRecordConfiguration : IEntityTypeConfiguration<VaccinationRecord>
{
    public void Configure(EntityTypeBuilder<VaccinationRecord> builder)
    {
        builder.HasKey(record => record.Id);

        builder.Property(record => record.Dose)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(record => record.ApplicationDate)
            .IsRequired();
    }
}

