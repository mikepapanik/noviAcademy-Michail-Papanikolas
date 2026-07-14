using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Persistence.Commands.Players;

public sealed class CreatePlayerPersistence
    : ICreatePlayerPersistence
{
    private readonly IPlayerRepository _playerRepository;

    public CreatePlayerPersistence(
        IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task CreateAsync(
        Player player,
        CancellationToken cancellationToken)
    {
        await _playerRepository.AddPlayerAsync(
            player,
            cancellationToken);
    }
}
