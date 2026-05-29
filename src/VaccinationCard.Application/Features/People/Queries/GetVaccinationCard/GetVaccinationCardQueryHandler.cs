using MediatR;
using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Features.People.Queries.GetVaccinationCard;

// Handler para as queries do cartão de vacina (lista registro de vacinas de uma pessoa)
public class GetVaccinationCardQueryHandler(IPersonRepository personRepository)
    : IRequestHandler<GetVaccinationCardQuery, VaccinationCardDto>
{
    private readonly IPersonRepository _personRepository = personRepository;

    public async Task<VaccinationCardDto> Handle(GetVaccinationCardQuery request, CancellationToken cancellationToken)
    {
        Person? person = await _personRepository.GetByIdWithRecordsAsync(request.PersonId, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException(nameof(Person), request.PersonId);
        }

        var entries = person.VaccinationRecords
            .OrderByDescending(record => record.ApplicationDate) // ordena pela mais recente
            .Select(record => new VaccinationCardEntryDto(
                record.Id,
                record.VaccineId,
                record.Vaccine?.Name ?? string.Empty,
                record.Dose,
                record.ApplicationDate))
            .ToList();

        return new VaccinationCardDto(
            person.Id,
            person.Name,
            person.IdentificationNumber,
            entries);
    }
}
