using MediatR;
using VaccinationCard.Application.Common.Interfaces;

namespace VaccinationCard.Application.Features.Vaccines.Queries.GetVaccines;

// Handler para a listagem de vacinas
public class GetVaccinesQueryHandler(IVaccineRepository vaccineRepository)
    : IRequestHandler<GetVaccinesQuery, IReadOnlyList<VaccineDto>>
{
    private readonly IVaccineRepository _vaccineRepository = vaccineRepository;

    public async Task<IReadOnlyList<VaccineDto>> Handle(GetVaccinesQuery request, CancellationToken cancellationToken)
    {
        var vaccines = await _vaccineRepository.GetAllAsync(cancellationToken);

        return vaccines
            .OrderBy(vaccine => vaccine.Name)
            .Select(vaccine => new VaccineDto(vaccine.Id, vaccine.Name))
            .ToList();
    }
}
