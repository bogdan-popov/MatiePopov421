using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class TransactionType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; } = new List<BalanceTransaction>();
}
