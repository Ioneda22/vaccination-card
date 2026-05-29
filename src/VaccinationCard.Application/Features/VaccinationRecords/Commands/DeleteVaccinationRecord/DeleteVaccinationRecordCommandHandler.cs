using MediatR;
using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Features.VaccinationRecords.Commands.DeleteVaccinationRecord;

// Handler para deleção de um registro de vacina no DB
public class DeleteVaccinationRecordCommandHandler(
    IVaccinationRecordRepository vaccinationRecordRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteVaccinationRecordCommand>
{
    private readonly IVaccinationRecordRepository _vaccinationRecordRepository = vaccinationRecordRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(DeleteVaccinationRecordCommand request, CancellationToken cancellationToken)
    {
        VaccinationRecord? record =
            await _vaccinationRecordRepository.GetByIdAsync(request.RecordId, cancellationToken);

        if (record is null)
        {
            throw new NotFoundException(nameof(VaccinationRecord), request.RecordId);
        }

        _vaccinationRecordRepository.Remove(record);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
