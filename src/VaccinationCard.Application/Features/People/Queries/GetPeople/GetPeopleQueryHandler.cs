using MediatR;
using VaccinationCard.Application.Common.Interfaces;

namespace VaccinationCard.Application.Features.People.Queries.GetPeople;

// Handler para a listagem de pessoas
public class GetPeopleQueryHandler(IPersonRepository personRepository)
    : IRequestHandler<GetPeopleQuery, IReadOnlyList<PersonSummaryDto>>
{
    private readonly IPersonRepository _personRepository = personRepository;

    public async Task<IReadOnlyList<PersonSummaryDto>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
    {
        var people = await _personRepository.GetAllAsync(cancellationToken);

        return people
            .OrderBy(person => person.Name)
            .Select(person => new PersonSummaryDto(person.Id, person.Name, person.IdentificationNumber))
            .ToList();
    }
}
