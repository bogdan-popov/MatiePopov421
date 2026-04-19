using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class Review
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public int Targettypeid { get; set; }

    public int Targetid { get; set; }

    public int Rating { get; set; }

    public string? Text { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Lastmodifiedat { get; set; }

    public virtual ReviewTargetType Targettype { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
