using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class QualificationStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<QualificationRequest> QualificationRequests { get; set; } = new List<QualificationRequest>();
}
