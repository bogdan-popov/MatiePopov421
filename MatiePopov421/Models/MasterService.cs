using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class MasterService
{
    public int Masterid { get; set; }

    public int Serviceid { get; set; }

    public DateTime Assignedat { get; set; }

    public virtual User Master { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
