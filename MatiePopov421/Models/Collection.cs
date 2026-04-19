using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class Collection
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
