using MediatR;
using VaccinationCard.Application.Common.Exceptions;
using VaccinationCard.Application.Common.Interfaces;
using VaccinationCard.Domain.Entities;

namespace VaccinationCard.Application.Features.People.Commands.CreatePerson;

// Handler da entidade Pessoa
public class CreatePersonCommandHandler(IPersonRepository personRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePersonCommand, Guid>
{
    private readonly IPersonRepository _personRepository = personRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        bool alreadyExists =
            await _personRepository.IdentificationNumberExistsAsync(request.IdentificationNumber, cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException(
                $"Já existe uma pessoa com o número de identificação '{request.IdentificationNumber}'.");
        }

        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IdentificationNumber = request.IdentificationNumber
        };

        await _personRepository.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return person.Id;
    }
}
