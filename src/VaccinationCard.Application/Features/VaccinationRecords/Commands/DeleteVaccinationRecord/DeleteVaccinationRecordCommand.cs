using MediatR;

namespace VaccinationCard.Application.Features.VaccinationRecords.Commands.DeleteVaccinationRecord;

public record DeleteVaccinationRecordCommand(Guid RecordId) 
    : IRequest;
