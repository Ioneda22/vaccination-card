namespace VaccinationCard.Domain.Entities;


// Entidade de registro de uma vacinação
public class VaccinationRecord
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public Guid VaccineId { get; set; }
    public string Dose { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }

    public Person Person { get; set; } = null!;
    public Vaccine Vaccine { get; set; } = null!;
}
