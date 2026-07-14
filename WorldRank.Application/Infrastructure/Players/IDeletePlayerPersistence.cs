namespace WorldRank.Application.Infrastructure.Players;

public interface IDeletePlayerPersistence
{
    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}
