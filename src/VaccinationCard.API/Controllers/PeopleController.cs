using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccinationCard.API.Contracts;
using VaccinationCard.Application.Features.People.Commands.CreatePerson;
using VaccinationCard.Application.Features.People.Commands.DeletePerson;
using VaccinationCard.Application.Features.People.Queries.GetPeople;
using VaccinationCard.Application.Features.People.Queries.GetVaccinationCard;

namespace VaccinationCard.API.Controllers;

[ApiController]
[Authorize]
[Route("api/people")]
public class PeopleController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePersonRequest request,
        CancellationToken cancellationToken)
    {
        Guid personId = await _sender.Send(
            new CreatePersonCommand(request.Name, request.IdentificationNumber),
            cancellationToken);

        return CreatedAtAction(nameof(GetVaccinationCard), new { personId }, new { id = personId });
    }

    // Lista todas as pessoas cadastradas
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var people = await _sender.Send(new GetPeopleQuery(), cancellationToken);
        return Ok(people);
    }

    /// Consulta o cartão de vacinação completo de uma pessoa (listas de registro de vacinações)
    [HttpGet("{personId:guid}/vaccination-card")]
    public async Task<IActionResult> GetVaccinationCard(
        Guid personId,
        CancellationToken cancellationToken)
    {
        var card = await _sender.Send(new GetVaccinationCardQuery(personId), cancellationToken);
        return Ok(card);
    }

    [HttpDelete("{personId:guid}")]
    public async Task<IActionResult> Delete(Guid personId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeletePersonCommand(personId), cancellationToken);
        return NoContent();
    }
}
