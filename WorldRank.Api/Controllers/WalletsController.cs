using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorldRank.Api.Dtos.Wallets;
using WorldRank.Application.Commands.Wallets;
using WorldRank.Application.Queries.Wallets;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Api.Controllers;

[ApiController]
[Route("wallets")]
public class WalletsController : ControllerBase
{
    private readonly ISender _sender;

    public WalletsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("player/{playerId:int}")]
    public async Task<IActionResult> GetAll(
        int playerId,
        CancellationToken cancellationToken)
    {
        var wallets = await _sender.Send(
            new GetWalletsByPlayerIdQuery(playerId),
            cancellationToken);

        var response = wallets
            .Select(wallet => new WalletResponse(
                wallet.Id,
                wallet.PlayerId,
                wallet.Currency,
                wallet.Balance,
                wallet.IsBlocked))
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var wallet = await _sender.Send(
            new GetWalletByIdQuery(id),
            cancellationToken);

        if (wallet is null)
        {
            return NotFound();
        }

        var response = new WalletResponse(
            wallet.Id,
            wallet.PlayerId,
            wallet.Currency,
            wallet.Balance,
            wallet.IsBlocked);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateWalletRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new CreateWalletCommand(
                    request.Id,
                    request.PlayerId,
                    request.Currency,
                    request.Balance),
                cancellationToken);

            var response = new WalletResponse(
                request.Id,
                request.PlayerId,
                request.Currency,
                request.Balance,
                false);

            return CreatedAtAction(
                nameof(GetById),
                new { id = request.Id },
                response);
        }
        catch (PlayerNotFoundException exception)
        {
            return NotFound(
                new { error = exception.Message });
        }
        catch (DuplicateWalletException exception)
        {
            return Conflict(
                new { error = exception.Message });
        }
        catch (WalletException exception)
        {
            return BadRequest(
                new { error = exception.Message });
        }
    }

    [HttpPost("{id:int}/deposit")]
    public async Task<IActionResult> Deposit(
        int id,
        [FromBody] DepositRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var wallet = await _sender.Send(
                new DepositWalletCommand(
                    id,
                    request.Amount),
                cancellationToken);

            var response = new WalletResponse(
                wallet.Id,
                wallet.PlayerId,
                wallet.Currency,
                wallet.Balance,
                wallet.IsBlocked);

            return Ok(response);
        }
        catch (WalletNotFoundException exception)
        {
            return NotFound(
                new { error = exception.Message });
        }
        catch (WalletException exception)
        {
            return BadRequest(
                new { error = exception.Message });
        }
    }

    [HttpPost("{id:int}/withdraw")]
    public async Task<IActionResult> Withdraw(
        int id,
        [FromBody] DepositRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new WithdrawWalletCommand(
                    id,
                    request.Amount),
                cancellationToken);

            return NoContent();
        }
        catch (WalletNotFoundException exception)
        {
            return NotFound(
                new { error = exception.Message });
        }
        catch (WalletException exception)
        {
            return BadRequest(
                new { error = exception.Message });
        }
    }

    [HttpPatch("{id:int}/block")]
    public async Task<IActionResult> Block(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new BlockWalletCommand(id),
                cancellationToken);

            return NoContent();
        }
        catch (WalletNotFoundException exception)
        {
            return NotFound(
                new { error = exception.Message });
        }
    }

    [HttpPatch("{id:int}/unblock")]
    public async Task<IActionResult> Unblock(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new UnblockWalletCommand(id),
                cancellationToken);

            return NoContent();
        }
        catch (WalletNotFoundException exception)
        {
            return NotFound(
                new { error = exception.Message });
        }
    }
}