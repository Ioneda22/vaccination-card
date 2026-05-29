using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Common.Interfaces;

public interface IPersonRepository
{   
    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Person?> GetByIdWithRecordsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IdentificationNumberExistsAsync(string identificationNumber, CancellationToken cancellationToken = default);

    Task AddAsync(Person person, CancellationToken cancellationToken = default);
    void Remove(Person person);
}
