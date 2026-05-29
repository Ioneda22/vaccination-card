using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Common.Interfaces;

public interface IVaccineRepository
{
    Task<Vaccine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Vaccine>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Vaccine vaccine, CancellationToken cancellationToken = default);
}
