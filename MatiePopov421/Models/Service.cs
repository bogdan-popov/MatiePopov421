using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class Service
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Imagepath { get; set; }

    public int Collectionid { get; set; }

    public int Typeid { get; set; }

    public decimal Price { get; set; }

    public DateTime Lastmodifiedat { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Collection Collection { get; set; } = null!;

    public virtual ICollection<MasterService> MasterServices { get; set; } = new List<MasterService>();

    public virtual ServiceType Type { get; set; } = null!;
}
