using System;
using System.Collections.Generic;
using System.Text;

using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Strategies;

/// <summary>Subtracts funds directly by setting the balance.</summary>
public class ForceSubtractFundsStrategy : IFundsStrategy
{
    public FundsOperation Operation => FundsOperation.ForceSubtract;

    public void Execute(Wallet wallet, decimal amount)
    {
        wallet.SetBalance(wallet.Balance - amount);
    }
}
