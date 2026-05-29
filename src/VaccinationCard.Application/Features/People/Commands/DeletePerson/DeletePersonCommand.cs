using MediatR;

namespace VaccinationCard.Application.Features.People.Commands.DeletePerson;

// Comando para excluir uma pessoa
public record DeletePersonCommand(Guid PersonId)
    : IRequest;
