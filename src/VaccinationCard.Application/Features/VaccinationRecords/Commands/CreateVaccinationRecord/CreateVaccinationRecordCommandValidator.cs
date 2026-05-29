using FluentValidation;
using VaccinationCard.Domain.Constants;

namespace VaccinationCard.Application.Features.VaccinationRecords.Commands.CreateVaccinationRecord;

public class CreateVaccinationRecordCommandValidator : AbstractValidator<CreateVaccinationRecordCommand>
{
    public CreateVaccinationRecordCommandValidator()
    {
        RuleFor(command => command.PersonId)
            .NotEmpty().WithMessage("O identificador da pessoa é obrigatório.");

        RuleFor(command => command.VaccineId)
            .NotEmpty().WithMessage("O identificador da vacina é obrigatório.");

        // A dose só deve aceitar os valores definidos em 'VaccineDoses' -> domínio
        RuleFor(command => command.Dose) 
            .NotEmpty().WithMessage("A dose é obrigatória.")
            .Must(dose => VaccineDoses.All.Contains(dose))
            .WithMessage(_ => $"Dose inválida. Os valores aceitos são: {string.Join(", ", VaccineDoses.All)}.");

        RuleFor(command => command.ApplicationDate)
            .NotEmpty().WithMessage("A data de aplicação é obrigatória.")
            .LessThanOrEqualTo(_ => DateTime.UtcNow)
            .WithMessage("A data de aplicação não pode ser uma data futura.");
    }
}
