using WorldRank.Domain.Enums;

namespace WorldRank.Domain.Wallets
{
	public interface IWallet
	{
		int PlayerId { get; }
		Currency Currency { get; }
		decimal Balance { get; }
		bool IsBlocked { get; }

		void Block();
		void Unblock();
		void SetBalance(decimal balance);
		void Deposit(decimal amount);
		void Withdraw(decimal amount);
	}
}
