using MediatR;

namespace VaccinationCard.Application.Features.Vaccines.Commands.CreateVaccine;

public record CreateVaccineCommand(string Name) 
    : IRequest<Guid>;
