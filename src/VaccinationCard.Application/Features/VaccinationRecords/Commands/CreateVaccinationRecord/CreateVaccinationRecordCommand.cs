using MediatR;

namespace VaccinationCard.Application.Features.VaccinationRecords.Commands.CreateVaccinationRecord;

// Comando para criar um registro de vacinação
public record CreateVaccinationRecordCommand(
    Guid PersonId,
    Guid VaccineId,
    string Dose,
    DateTime ApplicationDate) : IRequest<Guid>;
