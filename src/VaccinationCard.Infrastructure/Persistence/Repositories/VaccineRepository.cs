using Microsoft.EntityFrameworkCore;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Infrastructure.Persistence.Repositories;

public class VaccineRepository(ApplicationDbContext context) : IVaccineRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<Vaccine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Vaccines.FirstOrDefaultAsync(vaccine => vaccine.Id == id, cancellationToken);

    public Task<List<Vaccine>> GetAllAsync(CancellationToken cancellationToken = default) =>
    _context.Vaccines.AsNoTracking().ToListAsync(cancellationToken);


    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Vaccines.AnyAsync(vaccine => vaccine.Id == id, cancellationToken);


    public async Task AddAsync(Vaccine vaccine, CancellationToken cancellationToken = default) =>
        await _context.Vaccines.AddAsync(vaccine, cancellationToken);
}
