using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccinationCard.API.Contracts;
using VaccinationCard.Application.Features.Vaccines.Commands.CreateVaccine;
using VaccinationCard.Application.Features.Vaccines.Queries.GetVaccines;

namespace VaccinationCard.API.Controllers;

[ApiController]
[Authorize]
[Route("api/vaccines")]
public class VaccinesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVaccineRequest request,
        CancellationToken cancellationToken)
    {
        Guid vaccineId = await _sender.Send(new CreateVaccineCommand(request.Name), cancellationToken);
        return Created($"/api/vaccines/{vaccineId}", new { id = vaccineId });
    }

    // Lista todas as vacinas cadastradas
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var vaccines = await _sender.Send(new GetVaccinesQuery(), cancellationToken);
        return Ok(vaccines);
    }
}
