using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Infrastructure.Identity;
using VaccinationCard.Infrastructure.Persistence;
using VaccinationCard.Infrastructure.Persistence.Repositories;

namespace VaccinationCard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("VaccinationCardDb"));

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IVaccineRepository, VaccineRepository>();
        services.AddScoped<IVaccinationRecordRepository, VaccinationRecordRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
