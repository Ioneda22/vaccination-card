namespace VaccinationCard.Application.Common.Exceptions;

public class NotFoundException(string entityName, object key)
    : Exception($"\"{entityName}\" com identificador '{key}' não foi encontrado(a).");
