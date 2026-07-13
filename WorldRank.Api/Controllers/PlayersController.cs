using Microsoft.AspNetCore.Mvc;
using WorldRank.Api.Dtos.Players;
using WorldRank.Application.Services;
using WorldRank.Domain.Player;

namespace WorldRank.Api.Controllers;

[ApiController]
[Route("players")]
public class PlayersController : ControllerBase
{
    private readonly PlayerService _playerService;

    public PlayersController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var players = await _playerService
            .GetAllPlayersAsync(
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
    public async Task<IActionResult> GetById(int id,CancellationToken cancellationToken)
    {
        var player = await _playerService.FindPlayerAsync(id,cancellationToken);

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
    public async Task<IActionResult> Create([FromBody] CreatePlayerRequest request,CancellationToken cancellationToken)
    {
        var player = new Player(
            request.Id,
            request.Name);

        player.AddScore(request.Score);

        var createdPlayer = await _playerService
            .AddPlayerAsync(
                player,
                cancellationToken);

        var response = new PlayerResponse(
            createdPlayer.Id,
            createdPlayer.Name,
            createdPlayer.Score);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPlayer.Id },
            response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var player = await _playerService.FindPlayerAsync(id,cancellationToken);

        if (player is null)
        {
            return NotFound();
        }

        await _playerService.DeletePlayerAsync(id,cancellationToken);

        return NoContent();
    }
}