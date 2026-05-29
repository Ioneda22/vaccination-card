namespace VaccinationCard.Application.Common.Interfaces;


// TO DO
public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string userName);
}
