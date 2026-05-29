using FluentValidation;

namespace VaccinationCard.Application.Features.Vaccines.Commands.CreateVaccine;

public class CreateVaccineCommandValidator : AbstractValidator<CreateVaccineCommand>
{
    public CreateVaccineCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome da vacina é obrigatório.")
            .MaximumLength(250).WithMessage("O nome da vacina deve ter no máximo 200 caracteres.");
    }
}
