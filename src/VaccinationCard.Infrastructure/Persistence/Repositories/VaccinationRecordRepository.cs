using Microsoft.EntityFrameworkCore;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Infrastructure.Persistence.Repositories;

public class VaccinationRecordRepository(ApplicationDbContext context) : IVaccinationRecordRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<VaccinationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.VaccinationRecords.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(VaccinationRecord record, CancellationToken cancellationToken = default) =>
        await _context.VaccinationRecords.AddAsync(record, cancellationToken);

    public void Remove(VaccinationRecord record) => _context.VaccinationRecords.Remove(record);
}
