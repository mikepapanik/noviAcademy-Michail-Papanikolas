using System;
using System.Collections.Generic;
using System.Text;

using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Strategies;

/// <summary>Adds funds - deposit, bonus.</summary>
public class AddFundsStrategy : IFundsStrategy
{
    public FundsOperation Operation => FundsOperation.Add;

    public void Execute(Wallet wallet, decimal amount) => wallet.Deposit(amount);
}
