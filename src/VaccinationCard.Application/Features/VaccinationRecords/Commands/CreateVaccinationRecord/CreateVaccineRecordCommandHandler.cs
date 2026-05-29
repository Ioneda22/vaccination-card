using MediatR;
using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Features.VaccinationRecords.Commands.CreateVaccinationRecord;

// Handler para a criação de um registro de vacinação
public class CreateVaccinationRecordCommandHandler(
    IPersonRepository personRepository,
    IVaccineRepository vaccineRepository,
    IVaccinationRecordRepository vaccinationRecordRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateVaccinationRecordCommand, Guid>
{
    private readonly IPersonRepository _personRepository = personRepository;
    private readonly IVaccineRepository _vaccineRepository = vaccineRepository;
    private readonly IVaccinationRecordRepository _vaccinationRecordRepository = vaccinationRecordRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateVaccinationRecordCommand request, CancellationToken cancellationToken)
    {
        bool personExists = await _personRepository.ExistsAsync(request.PersonId, cancellationToken);
        if (!personExists)
        {
            throw new NotFoundException(nameof(Person), request.PersonId);
        }

        bool vaccineExists = await _vaccineRepository.ExistsAsync(request.VaccineId, cancellationToken);
        if (!vaccineExists)
        {
            throw new NotFoundException(nameof(Vaccine), request.VaccineId);
        }

        var vaccinationRecord = new VaccinationRecord
        {
            Id = Guid.NewGuid(),
            PersonId = request.PersonId,
            VaccineId = request.VaccineId,
            Dose = request.Dose,
            ApplicationDate = request.ApplicationDate
        };

        await _vaccinationRecordRepository.AddAsync(vaccinationRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return vaccinationRecord.Id;
    }
}
