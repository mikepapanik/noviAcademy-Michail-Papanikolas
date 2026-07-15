using MediatR;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Commands.Players;

public sealed class CreatePlayerCommandHandler
    : IRequestHandler<CreatePlayerCommand>
{
    private readonly ICreatePlayerPersistence _createPlayerPersistence;

    public CreatePlayerCommandHandler(
        ICreatePlayerPersistence createPlayerPersistence)
    {
        _createPlayerPersistence = createPlayerPersistence;
    }

    public async Task Handle(
        CreatePlayerCommand request,
        CancellationToken cancellationToken)
    {
        var player = new Player(
            request.Id,
            request.Name);

        player.AddScore(request.Score);

        await _createPlayerPersistence.CreateAsync(
            player,
            cancellationToken);
    }
}
