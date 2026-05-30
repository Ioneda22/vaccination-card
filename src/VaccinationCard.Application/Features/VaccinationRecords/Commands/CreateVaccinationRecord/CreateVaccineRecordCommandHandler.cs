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

        var existingDoses = person.VaccinationRecords
            .Where(record => record.VaccineId == request.VaccineId)
            .ToDictionary(record => record.Dose, record => record.ApplicationDate, StringComparer.OrdinalIgnoreCase);

        ValidateBusinessRules(request.Dose, request.ApplicationDate, existingDoses);

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
    // Cada dose subsequente exige a dose anterior registrada E uma data de aplicação não anterior a ela.
    private static void ValidateBusinessRules(
        string requestedDose,
        DateTime requestedDate,
        IReadOnlyDictionary<string, DateTime> existingDoses)
    {
        // não permitir a mesma dose da mesma vacina mais de uma vez.
        if (existingDoses.ContainsKey(requestedDose))
        {
            throw new ConflictException(
                "Esta pessoa já tomou essa dose para esta vacina.");
        }

        // exige a 1ª e não pode ser anterior à data dela.
        if (DoseEquals(requestedDose, VaccineDoses.Second))
        {
            if (!existingDoses.TryGetValue(VaccineDoses.First, out DateTime firstDate))
            {
                throw new ConflictException(
                    "Não é possível registrar a 2ª dose: a 1ª dose desta vacina ainda não foi registrada.");
            }

            if (requestedDate < firstDate)
            {
                throw new ConflictException(
                    "A data da 2ª dose não pode ser anterior à data da 1ª dose.");
            }
        }

        // exige a 2ª e não pode ser anterior à data dela.
        if (DoseEquals(requestedDose, VaccineDoses.Third))
        {
            if (!existingDoses.TryGetValue(VaccineDoses.Second, out DateTime secondDate))
            {
                throw new ConflictException(
                    "Não é possível registrar a 3ª dose: a 2ª dose desta vacina ainda não foi registrada.");
            }

            if (requestedDate < secondDate)
            {
                throw new ConflictException(
                    "A data da 3ª dose não pode ser anterior à data da 2ª dose.");
            }
        }

        // exige a dose única OU a 2ª dose, e não pode ser anterior à data dessa dose anterior.
        if (DoseEquals(requestedDose, VaccineDoses.Booster))
        {
            bool hasSingle = existingDoses.TryGetValue(VaccineDoses.Single, out DateTime singleDate);
            bool hasSecond = existingDoses.TryGetValue(VaccineDoses.Second, out DateTime secondDate);

            if (!hasSingle && !hasSecond)
            {
                throw new ConflictException(
                    "Não é possível registrar o reforço: é necessário ter a dose única ou a 2ª dose desta vacina registrada previamente.");
            }

            // a dose anterior mais recente (única ou 2ª), caso ambas existam.
            DateTime previousDate = hasSingle && hasSecond
                ? (singleDate > secondDate ? singleDate : secondDate)
                : (hasSingle ? singleDate : secondDate);

            if (requestedDate < previousDate)
            {
                throw new ConflictException(
                    "A data do reforço não pode ser anterior à data da dose anterior.");
            }
        }
    }

    private static bool DoseEquals(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
