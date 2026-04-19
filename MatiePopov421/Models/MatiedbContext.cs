using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MatiePopov421.Models;

public partial class MatiedbContext : DbContext
{
    public MatiedbContext(DbContextOptions<MatiedbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BalanceTransaction> BalanceTransactions { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<Collection> Collections { get; set; }

    public virtual DbSet<MasterService> MasterServices { get; set; }

    public virtual DbSet<QualificationRequest> QualificationRequests { get; set; }

    public virtual DbSet<QualificationStatus> QualificationStatuses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewTargetType> ReviewTargetTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BalanceTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("balance_transactions_pkey");

            entity.ToTable("balance_transactions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Balanceafter)
                .HasPrecision(12, 2)
                .HasColumnName("balanceafter");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.Typeid).HasColumnName("typeid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Type).WithMany(p => p.BalanceTransactions)
                .HasForeignKey(d => d.Typeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("balance_transactions_typeid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.BalanceTransactions)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("balance_transactions_userid_fkey");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bookings_pkey");

            entity.ToTable("bookings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bookingdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("bookingdate");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Lastmodifiedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastmodifiedat");
            entity.Property(e => e.Masterid).HasColumnName("masterid");
            entity.Property(e => e.Queuenumber).HasColumnName("queuenumber");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Statusid).HasColumnName("statusid");
            entity.Property(e => e.Totalprice)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("totalprice");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Master).WithMany(p => p.BookingMasters)
                .HasForeignKey(d => d.Masterid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookings_masterid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookings_serviceid_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookings_statusid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.BookingUsers)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookings_userid_fkey");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("booking_statuses_pkey");

            entity.ToTable("booking_statuses");

            entity.HasIndex(e => e.Name, "booking_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("collections_pkey");

            entity.ToTable("collections");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<MasterService>(entity =>
        {
            entity.HasKey(e => new { e.Masterid, e.Serviceid }).HasName("master_services_pkey");

            entity.ToTable("master_services");

            entity.Property(e => e.Masterid).HasColumnName("masterid");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Assignedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assignedat");

            entity.HasOne(d => d.Master).WithMany(p => p.MasterServices)
                .HasForeignKey(d => d.Masterid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("master_services_masterid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.MasterServices)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("master_services_serviceid_fkey");
        });

        modelBuilder.Entity<QualificationRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("qualification_requests_pkey");

            entity.ToTable("qualification_requests");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Approvedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approvedat");
            entity.Property(e => e.Approvedbyid).HasColumnName("approvedbyid");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Lastmodifiedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastmodifiedat");
            entity.Property(e => e.Masterid).HasColumnName("masterid");
            entity.Property(e => e.Requestdate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("requestdate");
            entity.Property(e => e.Statusid).HasColumnName("statusid");

            entity.HasOne(d => d.Approvedby).WithMany(p => p.QualificationRequestApprovedbies)
                .HasForeignKey(d => d.Approvedbyid)
                .HasConstraintName("qualification_requests_approvedbyid_fkey");

            entity.HasOne(d => d.Master).WithMany(p => p.QualificationRequestMasters)
                .HasForeignKey(d => d.Masterid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("qualification_requests_masterid_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.QualificationRequests)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("qualification_requests_statusid_fkey");
        });

        modelBuilder.Entity<QualificationStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("qualification_statuses_pkey");

            entity.ToTable("qualification_statuses");

            entity.HasIndex(e => e.Name, "qualification_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reviews_pkey");

            entity.ToTable("reviews");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Lastmodifiedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastmodifiedat");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Targetid).HasColumnName("targetid");
            entity.Property(e => e.Targettypeid).HasColumnName("targettypeid");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Targettype).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Targettypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_targettypeid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_userid_fkey");
        });

        modelBuilder.Entity<ReviewTargetType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("review_target_types_pkey");

            entity.ToTable("review_target_types");

            entity.HasIndex(e => e.Name, "review_target_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("services_pkey");

            entity.ToTable("services");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Collectionid).HasColumnName("collectionid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(500)
                .HasColumnName("imagepath");
            entity.Property(e => e.Lastmodifiedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastmodifiedat");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Typeid).HasColumnName("typeid");

            entity.HasOne(d => d.Collection).WithMany(p => p.Services)
                .HasForeignKey(d => d.Collectionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("services_collectionid_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Services)
                .HasForeignKey(d => d.Typeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("services_typeid_fkey");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_types_pkey");

            entity.ToTable("service_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transaction_types_pkey");

            entity.ToTable("transaction_types");

            entity.HasIndex(e => e.Name, "transaction_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("balance");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Fullname)
                .HasMaxLength(200)
                .HasColumnName("fullname");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(256)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roleid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
