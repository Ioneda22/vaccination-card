namespace VaccinationCard.API.Contracts;

// Contratos da API para requisições HTTP.
// São separados dos Commands do MediatR para que a interface externa evolua de forma independente da lógica de negócios.
public record CreatePersonRequest(string Name, string IdentificationNumber);

public record CreateVaccineRequest(string Name);

public record CreateVaccinationRecordRequest(
    Guid VaccineId,
    string Dose,
    DateTime ApplicationDate);

public record LoginRequest(string UserName, string Password);
