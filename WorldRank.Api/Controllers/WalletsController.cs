using Microsoft.AspNetCore.Mvc;
using WorldRank.Application.Services;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;

namespace WorldRank.Api.Controllers;

[ApiController]
[Route("api/players/{playerId:int}/wallets")]
public class WalletsController : ControllerBase
{
    private readonly WalletService _walletService;

    public WalletsController(WalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public IActionResult GetAll(int playerId)
    {
        var wallets = _walletService
            .GetAllWalletsByPlayerId(playerId);

        return Ok(wallets);
    }

    [HttpPost]
    public IActionResult Create(
        int playerId,
        [FromBody] Wallet wallet)
    {
        _walletService.AddWalletToPlayer(
            wallet.Id,
            playerId,
            wallet.Currency,
            wallet.Balance);

        return Ok(wallet);
    }

    [HttpPost("{currency}/deposit")]
    public IActionResult Deposit(
        int playerId,
        Currency currency,
        [FromBody] decimal amount)
    {
        _walletService.Deposit(
            playerId,
            currency,
            amount);

        return NoContent();
    }

    [HttpPost("{currency}/withdraw")]
    public IActionResult Withdraw(
        int playerId,
        Currency currency,
        [FromBody] decimal amount)
    {
        _walletService.Withdraw(
            playerId,
            currency,
            amount);

        return NoContent();
    }

    [HttpPatch("{currency}/block")]
    public IActionResult Block(
        int playerId,
        Currency currency)
    {
        _walletService.Block(
            playerId,
            currency);

        return NoContent();
    }

    [HttpPatch("{currency}/unblock")]
    public IActionResult Unblock(
        int playerId,
        Currency currency)
    {
        _walletService.Unblock(
            playerId,
            currency);

        return NoContent();
    }
}
