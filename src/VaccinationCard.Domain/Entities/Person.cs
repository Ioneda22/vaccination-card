namespace VaccinationCard.Domain.Entities;

// Entidade da pessoa
// Nota: o numero de identificação não é o ID para o BD porque possui um significado no 'mundo real' (o que não seria considerado uma boa prática)

public class Person
{
    public Guid Id { get; set; }
                                   
    public string Name { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty; 

    public ICollection<VaccinationRecord> VaccinationRecords { get; set; } = new List<VaccinationRecord>();
}
