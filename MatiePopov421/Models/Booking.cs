using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public int Serviceid { get; set; }

    public int Masterid { get; set; }

    public DateTime Bookingdate { get; set; }

    public int Statusid { get; set; }

    public int Queuenumber { get; set; }

    public decimal Totalprice { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Lastmodifiedat { get; set; }

    public virtual User Master { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual BookingStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
