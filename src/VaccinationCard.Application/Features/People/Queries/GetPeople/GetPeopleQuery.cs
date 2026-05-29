using MediatR;

namespace VaccinationCard.Application.Features.People.Queries.GetPeople;

// Query para listar todas as pessoas cadastradas
public record GetPeopleQuery() 
    : IRequest<IReadOnlyList<PersonSummaryDto>>;
