using MediatR;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Commands.Players;

public sealed class UpdatePlayerCommandHandler
    : IRequestHandler<UpdatePlayerCommand, bool>
{
    private readonly IUpdatePlayerPersistence _updatePlayerPersistence;

    public UpdatePlayerCommandHandler(
        IUpdatePlayerPersistence updatePlayerPersistence)
    {
        _updatePlayerPersistence = updatePlayerPersistence;
    }

    public async Task<bool> Handle(
        UpdatePlayerCommand request,
        CancellationToken cancellationToken)
    {
        var updatedPlayer = new Player(
            request.Id,
            request.Name);

        updatedPlayer.AddScore(request.Score);

        return await _updatePlayerPersistence.UpdateAsync(
            updatedPlayer,
            cancellationToken);
    }
}