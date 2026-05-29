using MediatR;

namespace VaccinationCard.Application.Features.Vaccines.Queries.GetVaccines;

// Query para listar todas as vacinas cadastradas
public record GetVaccinesQuery() 
    : IRequest<IReadOnlyList<VaccineDto>>;
