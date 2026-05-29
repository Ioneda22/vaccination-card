namespace VaccinationCard.Domain.Constants;

// Classe para a dose de vacinas
// Nota: não é um enumerável porque isso dificultaria a validação com o FluentValidation (enumeravel != string)

public static class VaccineDoses
{
    public const string Single = "Single";
    public const string First = "First";
    public const string Second = "Second";
    public const string Third = "Third";
    public const string Booster = "Booster";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Single, First, Second, Third, Booster
        };
}
