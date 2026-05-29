using FluentValidation;

namespace VaccinationCard.Application.Features.People.Commands.CreatePerson;

// Validação da criação de uma Pessoa
public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(250).WithMessage("O nome deve ter no máximo 250 caracteres.");
        

        RuleFor(command => command.IdentificationNumber)
            .NotEmpty().WithMessage("O número de identificação é obrigatório.")
            .MaximumLength(50).WithMessage("O número de identificação deve ter no máximo 50 caracteres.");
    }
}
