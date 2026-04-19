using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class QualificationRequest
{
    public int Id { get; set; }

    public int Masterid { get; set; }

    public DateTime Requestdate { get; set; }

    public int Statusid { get; set; }

    public int? Approvedbyid { get; set; }

    public DateTime? Approvedat { get; set; }

    public string? Comment { get; set; }

    public DateTime Lastmodifiedat { get; set; }

    public virtual User? Approvedby { get; set; }

    public virtual User Master { get; set; } = null!;

    public virtual QualificationStatus Status { get; set; } = null!;
}
