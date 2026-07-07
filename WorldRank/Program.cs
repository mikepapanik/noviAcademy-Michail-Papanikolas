using WorldRank;

IPlayerRepository playerRepository = new InMemoryPlayerRepository();
IWalletRepository walletRepository = new InMemoryWalletRepository();

while (true)
{
    Console.WriteLine("\n=== WorldRank Player Registry ===");
    Console.WriteLine("1. Add player");
    Console.WriteLine("2. List all players");
    Console.WriteLine("3. Find player by name");
    Console.WriteLine("4. Find player by id");
    Console.WriteLine("5. Delete player");
    Console.WriteLine("6. Group players by score");
    Console.WriteLine("7. Add wallet to player");
    Console.WriteLine("8. List wallets by player");
    Console.WriteLine("9. Deposit to wallet");
    Console.WriteLine("10. Withdraw from wallet");
    Console.WriteLine("11. Block wallet");
    Console.WriteLine("12. Unblock wallet");
    Console.WriteLine("0. Exit");
    Console.Write("> ");

    Action? action = Console.ReadLine() switch
    {
        "1" => AddPlayer,
        "2" => ListPlayers,
        "3" => FindPlayer,
        "4" => FindPlayerById,
        "5" => DeletePlayer,
        "6" => GroupPlayersByScore,
        "7" => AddWalletToPlayer,
        "8" => ListWalletsByPlayer,
        "9" => DepositToWallet,
        "10" => WithdrawFromWallet,
        "11" => BlockWallet,
        "12" => UnblockWallet,
        "0" => null,
        _ => () => Console.WriteLine("Unknown option.")
    };

    if (action is null)
        return;

    action();
}

void AddPlayer()
{
    Console.Write("Id: ");
    var idInput = Console.ReadLine();

    if (!int.TryParse(idInput, out var id))
    {
        Console.WriteLine("Id must be a whole number.");
        return;
    }

    Console.Write("Name: ");
    var name = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    Console.Write("Score: ");
    var scoreInput = Console.ReadLine();

    if (!int.TryParse(scoreInput, out var score))
    {
        Console.WriteLine("Score must be a whole number.");
        return;
    }

    var player = new Player(id, name, score);

    try
    {
        playerRepository.AddPlayer(player);
        Console.WriteLine("Player added successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine(ex.Message);
    }
}

void ListPlayers()
{
    if (!playerRepository.GetAllPlayers().Any())
    {
        Console.WriteLine("No players registered.");
        return;
    }

    foreach (var player in playerRepository.GetAllPlayers())
    {
        Console.WriteLine(player);
    }
}

void FindPlayer()
{
    Console.Write("Search by name: ");
    var term = Console.ReadLine() ?? string.Empty;

    var player = playerRepository
        .GetAllPlayers()
        .FirstOrDefault(p => p.Name.Equals(term, StringComparison.OrdinalIgnoreCase));

    if (player is null)
    {
        Console.WriteLine("No player found.");
        return;
    }

    Console.WriteLine(player);
}

void FindPlayerById()
{
    Console.Write("Player id: ");
    var idInput = Console.ReadLine();

    if (!int.TryParse(idInput, out var id))
    {
        Console.WriteLine("Id must be a whole number.");
        return;
    }

    var player = playerRepository.FindPlayer(id);

    if (player is null)
    {
        Console.WriteLine("No player found.");
        return;
    }

    Console.WriteLine(player);
}

void DeletePlayer()
{
    Console.Write("Player id to delete: ");
    var idInput = Console.ReadLine();

    if (!int.TryParse(idInput, out var id))
    {
        Console.WriteLine("Id must be a whole number.");
        return;
    }

    bool deleted = playerRepository.DeletePlayer(id);

    if (!deleted)
    {
        Console.WriteLine("No player found.");
        return;
    }

    Console.WriteLine("Player deleted successfully.");
}

void GroupPlayersByScore()
{
    var groups = playerRepository.GroupPlayersByScore();

    foreach (var group in groups)
    {
        Console.WriteLine($"\nScore: {group.Key}");

        foreach (var player in group)
        {
            Console.WriteLine(player);
        }
    }
}

void AddWalletToPlayer()
{
    Console.Write("Player id: ");
    var playerIdInput = Console.ReadLine();

    if (!int.TryParse(playerIdInput, out var playerId))
    {
        Console.WriteLine("Player id must be a whole number.");
        return;
    }

    var player = playerRepository.FindPlayer(playerId);

    if (player is null)
    {
        Console.WriteLine("Player does not exist.");
        return;
    }

    Console.WriteLine("Choose currency:");
    Console.WriteLine("1. EUR");
    Console.WriteLine("2. USD");
    Console.WriteLine("3. GBP");
    Console.Write("> ");

    var currencyInput = Console.ReadLine();

    if (!int.TryParse(currencyInput, out var currencyValue) ||
        !Enum.IsDefined(typeof(Currency), currencyValue))
    {
        Console.WriteLine("Invalid currency.");
        return;
    }

    var currency = (Currency)currencyValue;
    var wallet = new Wallet(playerId, currency);

    try
    {
        walletRepository.Add(wallet, playerId);
        Console.WriteLine("Wallet added successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine(ex.Message);
    }
}

void ListWalletsByPlayer()
{
    Console.Write("Player id: ");
    var playerIdInput = Console.ReadLine();

    if (!int.TryParse(playerIdInput, out var playerId))
    {
        Console.WriteLine("Player id must be a whole number.");
        return;
    }

    var wallets = walletRepository.GetByPlayer(playerId).ToList();

    if (!wallets.Any())
    {
        Console.WriteLine("No wallets found for this player.");
        return;
    }

    foreach (var wallet in wallets)
    {
        Console.WriteLine(wallet);
    }
}

void DepositToWallet()
{
    Console.Write("Player id: ");
    var playerIdInput = Console.ReadLine();

    if (!int.TryParse(playerIdInput, out var playerId))
    {
        Console.WriteLine("Player id must be a whole number.");
        return;
    }

    var wallet = ChooseWallet(playerId);

    if (wallet is null)
        return;

    Console.Write("Amount to deposit: ");
    var amountInput = Console.ReadLine();

    if (!decimal.TryParse(amountInput, out var amount))
    {
        Console.WriteLine("Amount must be a number.");
        return;
    }

    try
    {
        wallet.Deposit(amount);
        Console.WriteLine("Deposit completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

void WithdrawFromWallet()
{
    Console.Write("Player id: ");
    var playerIdInput = Console.ReadLine();

    if (!int.TryParse(playerIdInput, out var playerId))
    {
        Console.WriteLine("Player id must be a whole number.");
        return;
    }

    var wallet = ChooseWallet(playerId);

    if (wallet is null)
        return;

    Console.Write("Amount to withdraw: ");
    var amountInput = Console.ReadLine();

    if (!decimal.TryParse(amountInput, out var amount))
    {
        Console.WriteLine("Amount must be a number.");
        return;
    }

    try
    {
        wallet.Withdraw(amount);
        Console.WriteLine("Withdraw completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

void BlockWallet()
{
    Console.Write("Player id: ");
    var playerIdInput = Console.ReadLine();

    if (!int.TryParse(playerIdInput, out var playerId))
    {
        Console.WriteLine("Player id must be a whole number.");
        return;
    }

    var wallet = ChooseWallet(playerId);

    if (wallet is null)
        return;

    wallet.Block();
    Console.WriteLine("Wallet blocked successfully.");
}

void UnblockWallet()
{
    Console.Write("Player id: ");
    var playerIdInput = Console.ReadLine();

    if (!int.TryParse(playerIdInput, out var playerId))
    {
        Console.WriteLine("Player id must be a whole number.");
        return;
    }

    var wallet = ChooseWallet(playerId);

    if (wallet is null)
        return;

    wallet.Unblock();
    Console.WriteLine("Wallet unblocked successfully.");
}

Wallet? ChooseWallet(int playerId)
{
    var wallets = walletRepository.GetByPlayer(playerId).ToList();

    if (!wallets.Any())
    {
        Console.WriteLine("No wallets found for this player.");
        return null;
    }

    for (int i = 0; i < wallets.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {wallets[i]}");
    }

    Console.Write("Choose wallet: ");
    var choiceInput = Console.ReadLine();

    if (!int.TryParse(choiceInput, out var choice) ||
        choice < 1 ||
        choice > wallets.Count)
    {
        Console.WriteLine("Invalid wallet choice.");
        return null;
    }

    return wallets[choice - 1];
}