using Microsoft.EntityFrameworkCore;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Infrastructure.Persistence.Repositories;

public class PersonRepository(ApplicationDbContext context) 
    : IPersonRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.People.FirstOrDefaultAsync(person => person.Id == id, cancellationToken);

    public Task<Person?> GetByIdWithRecordsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.People
            .Include(person => person.VaccinationRecords)
                .ThenInclude(record => record.Vaccine)
            .FirstOrDefaultAsync(person => person.Id == id, cancellationToken);

    public Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.People.AsNoTracking().ToListAsync(cancellationToken);


    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.People.AnyAsync(person => person.Id == id, cancellationToken);

    public Task<bool> IdentificationNumberExistsAsync(string identificationNumber, CancellationToken cancellationToken = default) =>
        _context.People.AnyAsync(person => person.IdentificationNumber == identificationNumber, cancellationToken);


    public async Task AddAsync(Person person, CancellationToken cancellationToken = default) =>
        await _context.People.AddAsync(person, cancellationToken);


    public void Remove(Person person) => _context.People.Remove(person);
}
