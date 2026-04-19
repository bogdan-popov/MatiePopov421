using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class BookingStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
