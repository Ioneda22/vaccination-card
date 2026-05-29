using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Common.Interfaces;

public interface IVaccinationRecordRepository
{
    Task<VaccinationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(VaccinationRecord record, CancellationToken cancellationToken = default);
    
    void Remove(VaccinationRecord record);
}
