using Microsoft.Extensions.DependencyInjection;
using NLog;
using WorldRank.Application.Services;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Player;
using WorldRank.Domain.Wallets;

var services = new ServiceCollection();

WorldRank.DependencyInjection.AddWorldRank(services);

using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

var logger = LogManager.GetCurrentClassLogger();

var playerService = scope.ServiceProvider
    .GetRequiredService<PlayerService>();

var walletService = scope.ServiceProvider
    .GetRequiredService<WalletService>();

logger.Info("Application started.");

while (true)
{
    Console.WriteLine("\n=== WorldRank Player Registry ===");
    Console.WriteLine("--- Players ---");
    Console.WriteLine("1. Add player");
    Console.WriteLine("2. List all players");
    Console.WriteLine("3. List players grouped by score");
    Console.WriteLine("4. Find player by name");
    Console.WriteLine("5. Find player by id");
    Console.WriteLine("6. Delete player");
    Console.WriteLine("--- Wallets ---");
    Console.WriteLine("7. Add wallet to player");
    Console.WriteLine("8. Show player wallets");
    Console.WriteLine("9. Deposit to wallet");
    Console.WriteLine("10. Withdraw from wallet");
    Console.WriteLine("11. Block wallet");
    Console.WriteLine("12. Unblock wallet");
    Console.WriteLine("13. Update wallet balance");
    Console.WriteLine("0. Exit");
    Console.Write("> ");

    Func<Task>? action = Console.ReadLine() switch
    {
        "1" => AddPlayerAsync,
        "2" => ListPlayersAsync,
        "3" => ListPlayersByScoreAsync,
        "4" => FindPlayerByNameAsync,
        "5" => FindPlayerByIdAsync,
        "6" => DeletePlayerAsync,
        "7" => AddWalletToPlayerAsync,
        "8" => GetWalletsOfPlayerAsync,
        "9" => DepositToWalletAsync,
        "10" => WithdrawFromWalletAsync,
        "11" => BlockWalletAsync,
        "12" => UnblockWalletAsync,
        "13" => UpdateWalletBalanceAsync,
        "0" => null,
        _ => () =>
        {
            Console.WriteLine("Unknown option.");
            return Task.CompletedTask;
        }
    };

    if (action is null)
    {
        logger.Info("Application exiting.");
        LogManager.Shutdown();
        return;
    }

    try
    {
        await action();
    }
    catch (Exception ex)
    {
        logger.Error(
            ex,
            "Unexpected error while handling a menu action");

        Console.WriteLine(
            $"Unexpected error: {ex.Message}");
    }
}

#region Input Helpers

int? PromptPlayerId()
{
    Console.Write("Give player id: ");

    if (int.TryParse(
        Console.ReadLine(),
        out var playerId))
    {
        return playerId;
    }

    Console.WriteLine(
        "Player id must be a whole number.");

    return null;
}

int? PromptWalletId()
{
    Console.Write("Give wallet id: ");

    if (int.TryParse(
        Console.ReadLine(),
        out var walletId))
    {
        return walletId;
    }

    Console.WriteLine(
        "Wallet id must be a whole number.");

    return null;
}

Currency? PromptCurrency()
{
    Console.WriteLine(
        "Give Currency: 1 - EUR | 2 - USD");

    switch (Console.ReadLine())
    {
        case "1":
            return Currency.EUR;

        case "2":
            return Currency.USD;

        default:
            Console.WriteLine("Unknown currency.");
            return null;
    }
}

decimal? PromptAmount(string label)
{
    Console.Write($"{label}: ");

    if (decimal.TryParse(
        Console.ReadLine(),
        out var amount))
    {
        return amount;
    }

    Console.WriteLine(
        "Amount must be a number.");

    return null;
}

async Task<int> GeneratePlayerIdAsync()
{
    var existingIds = (await playerService
        .GetAllPlayersAsync(
            CancellationToken.None))
        .Select(player => player.Id)
        .ToHashSet();

    int id;

    do
    {
        id = Random.Shared.Next(
            1,
            int.MaxValue);
    }
    while (existingIds.Contains(id));

    return id;
}

async Task<int> GenerateWalletIdAsync()
{
    var players = await playerService
        .GetAllPlayersAsync(
            CancellationToken.None);

    var walletLists = await Task.WhenAll(
        players.Select(player =>
            walletService.GetAllWalletsByPlayerIdAsync(
                player.Id,
                CancellationToken.None)));

    var existingWalletIds = walletLists
        .SelectMany(wallets => wallets)
        .Select(wallet => wallet.Id)
        .ToHashSet();

    int id;

    do
    {
        id = Random.Shared.Next(
            1,
            int.MaxValue);
    }
    while (existingWalletIds.Contains(id));

    return id;
}

async Task RunWalletOperationAsync(
    Func<Task> operation)
{
    try
    {
        await operation();
    }
    catch (WalletException ex)
    {
        logger.Warn(
            ex,
            "Wallet operation failed");

        Console.WriteLine(
            $"Error: {ex.Message}");
    }
}

#endregion Input Helpers

#region Player Methods

async Task AddPlayerAsync()
{
    Console.Write("Name: ");
    var name = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine(
            "Name cannot be empty.");

        return;
    }

    Console.Write("Score: ");
    var scoreInput = Console.ReadLine();

    if (!int.TryParse(
        scoreInput,
        out var score))
    {
        Console.WriteLine(
            "Score must be a whole number.");

        return;
    }

    var player = new Player(
        await GeneratePlayerIdAsync(),
        name);

    player.AddScore(score);

    await playerService.AddPlayerAsync(
        player,
        CancellationToken.None);

    Console.WriteLine(
        "Player added successfully.");
}

async Task ListPlayersAsync()
{
    var all = (await playerService
        .GetAllPlayersAsync(
            CancellationToken.None))
        .ToList();

    if (all.Count == 0)
    {
        Console.WriteLine(
            "No players registered.");

        return;
    }

    foreach (var player in all)
    {
        Console.WriteLine(player);
    }
}

async Task ListPlayersByScoreAsync()
{
    var groups = (await playerService
        .GroupPlayersByScoreAsync(
            CancellationToken.None))
        .ToList();

    if (groups.Count == 0)
    {
        Console.WriteLine(
            "No players registered.");

        return;
    }

    foreach (var group in groups)
    {
        Console.WriteLine(
            $"Score {group.Key}:");

        foreach (var player in group)
        {
            Console.WriteLine(
                $"  {player}");
        }
    }
}

async Task FindPlayerByNameAsync()
{
    Console.Write("Search by name: ");

    var term =
        Console.ReadLine() ?? string.Empty;

    var player = (await playerService
        .GetAllPlayersAsync(
            CancellationToken.None))
        .FirstOrDefault(player =>
            player.Name.Equals(
                term,
                StringComparison.OrdinalIgnoreCase));

    Console.WriteLine(
        player is null
            ? "No player found."
            : player.ToString());
}

async Task FindPlayerByIdAsync()
{
    var playerId = PromptPlayerId();

    if (playerId is null)
    {
        return;
    }

    var player = await playerService
        .FindPlayerAsync(
            playerId.Value,
            CancellationToken.None);

    Console.WriteLine(
        player is null
            ? "No player found."
            : player.ToString());
}

async Task DeletePlayerAsync()
{
    var playerId = PromptPlayerId();

    if (playerId is null)
    {
        return;
    }

    await playerService.DeletePlayerAsync(
        playerId.Value,
        CancellationToken.None);

    Console.WriteLine(
        "Player deleted (if it existed).");
}

#endregion Player Methods

#region Wallet Methods

async Task AddWalletToPlayerAsync()
{
    var playerId = PromptPlayerId();

    if (playerId is null)
    {
        return;
    }

    var currency = PromptCurrency();

    if (currency is null)
    {
        return;
    }

    var balance =
        PromptAmount("Initial balance");

    if (balance is null)
    {
        return;
    }

    try
    {
        var walletId =
            await GenerateWalletIdAsync();

        await walletService.AddWalletToPlayerAsync(
            walletId,
            playerId.Value,
            currency.Value,
            balance.Value,
            CancellationToken.None);

        Console.WriteLine(
            "Wallet added successfully.");
    }
    catch (PlayerNotFoundException ex)
    {
        logger.Warn(
            ex,
            "Could not add wallet, player {PlayerId} not found",
            playerId.Value);

        Console.WriteLine(
            $"Error: {ex.Message}");
    }
    catch (WalletException ex)
    {
        logger.Warn(
            ex,
            "Could not add wallet for player {PlayerId} in {Currency}",
            playerId.Value,
            currency.Value);

        Console.WriteLine(
            $"Error: {ex.Message}");
    }
}

async Task GetWalletsOfPlayerAsync()
{
    var playerId = PromptPlayerId();

    if (playerId is null)
    {
        return;
    }

    var wallets = (await walletService
        .GetAllWalletsByPlayerIdAsync(
            playerId.Value,
            CancellationToken.None))
        .ToList();

    if (wallets.Count == 0)
    {
        Console.WriteLine(
            "No wallets found for this player.");

        return;
    }

    for (var index = 0;
         index < wallets.Count;
         index++)
    {
        Console.WriteLine(
            $"Wallet Number {index + 1} {wallets[index]}");
    }
}

async Task DepositToWalletAsync()
{
    var walletId = PromptWalletId();

    if (walletId is null)
    {
        return;
    }

    var amount =
        PromptAmount("Amount to deposit");

    if (amount is null)
    {
        return;
    }

    await RunWalletOperationAsync(async () =>
    {
        await walletService.DepositAsync(
            walletId.Value,
            amount.Value,
            CancellationToken.None);

        Console.WriteLine(
            "Deposit successful.");
    });
}

async Task WithdrawFromWalletAsync()
{
    var walletId = PromptWalletId();

    if (walletId is null)
    {
        return;
    }

    var amount =
        PromptAmount("Amount to withdraw");

    if (amount is null)
    {
        return;
    }

    await RunWalletOperationAsync(async () =>
    {
        await walletService.WithdrawAsync(
            walletId.Value,
            amount.Value,
            CancellationToken.None);

        Console.WriteLine(
            "Withdrawal successful.");
    });
}

async Task BlockWalletAsync()
{
    var walletId = PromptWalletId();

    if (walletId is null)
    {
        return;
    }

    await RunWalletOperationAsync(async () =>
    {
        await walletService.BlockAsync(
            walletId.Value,
            CancellationToken.None);

        Console.WriteLine(
            "Wallet blocked.");
    });
}

async Task UnblockWalletAsync()
{
    var walletId = PromptWalletId();

    if (walletId is null)
    {
        return;
    }

    await RunWalletOperationAsync(async () =>
    {
        await walletService.UnblockAsync(
            walletId.Value,
            CancellationToken.None);

        Console.WriteLine(
            "Wallet unblocked.");
    });
}

async Task UpdateWalletBalanceAsync()
{
    var walletId = PromptWalletId();

    if (walletId is null)
    {
        return;
    }

    var newBalance =
        PromptAmount("New balance");

    if (newBalance is null)
    {
        return;
    }

    await RunWalletOperationAsync(async () =>
    {
        await walletService.UpdateBalanceAsync(
            walletId.Value,
            newBalance.Value,
            CancellationToken.None);

        Console.WriteLine(
            "Balance updated.");
    });
}

#endregion Wallet Methods