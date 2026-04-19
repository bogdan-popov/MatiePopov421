using System;
using System.Collections.Generic;

namespace MatiePopov421.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Fullname { get; set; }

    public int Roleid { get; set; }

    public decimal Balance { get; set; }

    public bool Isactive { get; set; }

    public DateTime Createdat { get; set; }

    public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; } = new List<BalanceTransaction>();

    public virtual ICollection<Booking> BookingMasters { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingUsers { get; set; } = new List<Booking>();

    public virtual ICollection<MasterService> MasterServices { get; set; } = new List<MasterService>();

    public virtual ICollection<QualificationRequest> QualificationRequestApprovedbies { get; set; } = new List<QualificationRequest>();

    public virtual ICollection<QualificationRequest> QualificationRequestMasters { get; set; } = new List<QualificationRequest>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;
}
