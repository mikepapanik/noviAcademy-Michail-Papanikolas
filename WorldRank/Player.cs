using System;

namespace WorldRank;

public class Player : IPlayer
{
	public int Id { get; }
	public string Name { get; }
	public int Score { get; private set; }

	public Player(int id, string name, int score)
	{
		if (id <= 0)
			throw new ArgumentException("Id must be positive.", nameof(id));

		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be empty.", nameof(name));

		if (score < 0)
			throw new ArgumentOutOfRangeException(nameof(score), "Score cannot be negative.");

		Id = id;
		Name = name;
		Score = score;
	}

	public void UpdateScore(int newScore)
	{
		if (newScore < 0)
			throw new ArgumentOutOfRangeException(nameof(newScore), "Score cannot be negative.");

		Score = newScore;
	}

	public override string ToString()
	{
		return $"Id: {Id} | Name: {Name} | Score: {Score}";
	}
}