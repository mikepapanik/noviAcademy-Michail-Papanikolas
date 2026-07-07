namespace WorldRank;

public interface IPlayer
{
    int Id { get; }
    string Name { get; }
    int Score { get; }

    void UpdateScore(int newScore);
}