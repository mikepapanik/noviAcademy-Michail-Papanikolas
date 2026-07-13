using Microsoft.AspNetCore.Mvc;
using WorldRank.Application.Services;
using WorldRank.Domain.Player;

namespace WorldRank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly PlayerService _playerService;

    public PlayersController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_playerService.GetAllPlayers());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var player = _playerService.FindPlayer(id);

        if (player is null)
        {
            return NotFound();
        }

        return Ok(player);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Player player)
    {
        _playerService.AddPlayer(player);

        return CreatedAtAction(
            nameof(GetById),
            new { id = player.Id },
            player);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var player = _playerService.FindPlayer(id);

        if (player is null)
        {
            return NotFound();
        }

        _playerService.DeletePlayer(id);

        return NoContent();
    }
}