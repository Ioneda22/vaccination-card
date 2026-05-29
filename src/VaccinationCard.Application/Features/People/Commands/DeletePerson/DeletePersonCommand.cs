using MediatR;

namespace VaccinationCard.Application.Features.People.Commands.CreatePerson;

// Comando para excluir uma pessoa
public record DeletePersonCommand(Guid PersonId)
    : IRequest;
