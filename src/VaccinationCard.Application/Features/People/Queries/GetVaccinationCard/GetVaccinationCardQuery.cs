using MediatR;

namespace VaccinationCard.Application.Features.People.Queries.GetVaccinationCard;

public record GetVaccinationCardQuery(Guid PersonId) 
    : IRequest<VaccinationCardDto>;
