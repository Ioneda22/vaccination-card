namespace VaccinationCard.Application.Features.People.Queries.GetVaccinationCard;

// DTOs de request e output para facilitar as queries
public record VaccinationCardDto(
    Guid PersonId,
    string PersonName,
    string IdentificationNumber,
    IReadOnlyList<VaccinationCardEntryDto> Vaccinations);

public record VaccinationCardEntryDto(
    Guid RecordId,
    Guid VaccineId,
    string VaccineName,
    string Dose,
    DateTime ApplicationDate);
