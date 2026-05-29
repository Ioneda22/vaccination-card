using MediatR;
using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Application.Features.People.Commands.CreatePerson;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Features.People.Commands.DeletePerson;

// Handler para o comando de deletar uma pessoa do DB
public class DeletePersonCommandHandler(IPersonRepository personRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePersonCommand>
{
    private readonly IPersonRepository _personRepository = personRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        Person? person = await _personRepository.GetByIdWithRecordsAsync(request.PersonId, cancellationToken);
        
        if (person is null)
        {
            throw new NotFoundException(nameof(Person), request.PersonId);
        }

        _personRepository.Remove(person);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
