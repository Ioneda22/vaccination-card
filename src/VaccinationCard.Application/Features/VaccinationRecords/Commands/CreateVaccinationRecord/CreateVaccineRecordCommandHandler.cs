using MediatR;
using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Constants;
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
        // Carrega a pessoa com o histórico de vacinação para validar as regras de negócio em memória.
        Person? person = await _personRepository.GetByIdWithRecordsAsync(request.PersonId, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException(nameof(Person), request.PersonId);
        }

        bool vaccineExists = await _vaccineRepository.ExistsAsync(request.VaccineId, cancellationToken);
        if (!vaccineExists)
        {
            throw new NotFoundException(nameof(Vaccine), request.VaccineId);
        }

        // Doses que a pessoa já possui PARA ESTA vacina
        var existingDoses = person.VaccinationRecords
            .Where(record => record.VaccineId == request.VaccineId)
            .Select(record => record.Dose)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        ValidateBusinessRules(request.Dose, existingDoses);

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

    // Valida as regras de negócio do histórico de doses (por vacina) antes de registrar uma nova dose.
    private static void ValidateBusinessRules(string requestedDose, IReadOnlySet<string> existingDoses)
    {
        // não permitir a mesma dose da mesma vacina mais de uma vez.
        if (existingDoses.Contains(requestedDose))
        {
            throw new ConflictException(
                $"Esta pessoa já tomou essa dose para esta vacina.");
        }

        // cada dose exige a anterior, para a mesma vacina
        if (Equals(requestedDose, VaccineDoses.Second) && !existingDoses.Contains(VaccineDoses.First))
        {
            throw new ConflictException(
                "Não é possível registrar a 2ª dose: a 1ª dose desta vacina ainda não foi registrada.");
        }

        if (Equals(requestedDose, VaccineDoses.Third) && !existingDoses.Contains(VaccineDoses.Second))
        {
            throw new ConflictException(
                "Não é possível registrar a 3ª dose: a 2ª dose desta vacina ainda não foi registrada.");
        }

        // Reforço exige ao menos a dose única ou a 2ª dose previamente registrada.
        if (Equals(requestedDose, VaccineDoses.Booster)
            && !existingDoses.Contains(VaccineDoses.Single)
            && !existingDoses.Contains(VaccineDoses.Second))
        {
            throw new ConflictException(
                "Não é possível registrar o reforço: é necessário ter a dose única ou a 2ª dose desta vacina registrada previamente.");
        }
    }

    private static bool Equals(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
