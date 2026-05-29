using MediatR;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Features.People.Commands.CreatePerson;

// Comando para criar uma pessoa -> retorna o GUID
public record CreatePersonCommand(string Name, string IdentificationNumber) 
    : IRequest<Guid>;
