using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class BalanceTransaction
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public decimal Amount { get; set; }

    public int Typeid { get; set; }

    public string? Description { get; set; }

    public decimal Balanceafter { get; set; }

    public DateTime Createdat { get; set; }

    public virtual TransactionType Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
