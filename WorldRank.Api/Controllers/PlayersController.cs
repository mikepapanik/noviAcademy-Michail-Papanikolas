using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorldRank.Api.Dtos.Players;
using WorldRank.Application.Commands.Players;
using WorldRank.Application.Queries.Players;

namespace WorldRank.Api.Controllers;

[ApiController]
[Route("players")]
public class PlayersController : ControllerBase
{
    private readonly ISender _sender;

    public PlayersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var players = await _sender.Send(
            new GetAllPlayersQuery(),
            cancellationToken);

        var response = players
            .Select(player => new PlayerResponse(
                player.Id,
                player.Name,
                player.Score))
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var player = await _sender.Send(
            new GetPlayerByIdQuery(id),
            cancellationToken);

        if (player is null)
        {
            return NotFound();
        }

        var response = new PlayerResponse(
            player.Id,
            player.Name,
            player.Score);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePlayerRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new CreatePlayerCommand(
                request.Id,
                request.Name,
                request.Score),
            cancellationToken);

        var response = new PlayerResponse(
            request.Id,
            request.Name,
            request.Score);

        return CreatedAtAction(
            nameof(GetById),
            new { id = request.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdatePlayerRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(
            new UpdatePlayerCommand(
                id,
                request.Name,
                request.Score),
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        var response = new PlayerResponse(
            id,
            request.Name,
            request.Score);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(
            new DeletePlayerCommand(id),
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}