using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccinationCard.API.Contracts;
using VaccinationCard.Application.Features.VaccinationRecords.Commands.CreateVaccinationRecord;
using VaccinationCard.Application.Features.VaccinationRecords.Commands.DeleteVaccinationRecord;

namespace VaccinationCard.API.Controllers;

[ApiController]
[Authorize]
[Route("api/people/{personId:guid}/vaccination-records")]
public class VaccinationRecordsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    // Registra uma vacinação no cartão de uma pessoa
    [HttpPost]
    public async Task<IActionResult> Create(
        Guid personId,
        [FromBody] CreateVaccinationRecordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateVaccinationRecordCommand(
            personId,
            request.VaccineId,
            request.Dose,
            request.ApplicationDate);

        Guid recordId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(Create),
            routeValues: new { personId, recordId },
            value: new { id = recordId });
    }

    // Exclui um registro de vacinação específico
    [HttpDelete("{recordId:guid}")]
    public async Task<IActionResult> Delete(
        Guid recordId,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteVaccinationRecordCommand(recordId), cancellationToken);
        return NoContent();
    }
}
