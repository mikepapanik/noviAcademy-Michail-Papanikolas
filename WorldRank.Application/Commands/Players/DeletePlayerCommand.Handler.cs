using MediatR;
using WorldRank.Application.Infrastructure.Players;

namespace WorldRank.Application.Commands.Players;

public sealed class DeletePlayerCommandHandler
    : IRequestHandler<DeletePlayerCommand, bool>
{
    private readonly IDeletePlayerPersistence _deletePlayerPersistence;

    public DeletePlayerCommandHandler(
        IDeletePlayerPersistence deletePlayerPersistence)
    {
        _deletePlayerPersistence = deletePlayerPersistence;
    }

    public async Task<bool> Handle(
        DeletePlayerCommand request,
        CancellationToken cancellationToken)
    {
        return await _deletePlayerPersistence.DeleteAsync(
            request.Id,
            cancellationToken);
    }
}
