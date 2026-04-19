using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class ReviewTargetType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
