namespace VaccinationCard.Domain.Entities;

// Entidade da vacina
public class Vaccine
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<VaccinationRecord> VaccinationRecords { get; set; } = new List<VaccinationRecord>();
}
