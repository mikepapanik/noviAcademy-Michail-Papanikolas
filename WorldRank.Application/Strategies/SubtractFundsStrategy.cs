using System;
using System.Collections.Generic;
using System.Text;

using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Strategies;

/// <summary>Subtracts funds using normal withdraw rules.</summary>
public class SubtractFundsStrategy : IFundsStrategy
{
    public FundsOperation Operation => FundsOperation.Subtract;

    public void Execute(Wallet wallet, decimal amount)
    {
        wallet.Withdraw(amount);
    }
}
