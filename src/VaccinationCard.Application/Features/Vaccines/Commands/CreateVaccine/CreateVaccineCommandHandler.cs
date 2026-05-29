using MediatR;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Features.Vaccines.Commands.CreateVaccine;

// Handler para a criação de uma vacina
public class CreateVaccineCommandHandler(IVaccineRepository vaccineRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateVaccineCommand, Guid>
{
    private readonly IVaccineRepository _vaccineRepository = vaccineRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateVaccineCommand request, CancellationToken cancellationToken)
    {
        var vaccine = new Vaccine
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _vaccineRepository.AddAsync(vaccine, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return vaccine.Id;
    }
}
