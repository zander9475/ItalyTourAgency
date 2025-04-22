using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ItalyTourAgency.Models;

public class ItalyContext : IdentityDbContext<User>
{
    public ItalyContext(DbContextOptions<ItalyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<TourInstance> TourInstances { get; set; }

    public virtual DbSet<TourLocation> TourLocations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Booking_pk");

            entity.ToTable("Booking");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("card_number");
            entity.Property(e => e.GroupSize).HasColumnName("group_size");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentProcessed).HasColumnName("payment_processed");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("total_price");
            entity.Property(e => e.TourInstanceId).HasColumnName("tour_instance_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.TourInstance).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TourInstanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Booking_Tour_Instance");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_Bookings_User");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Location_pk");

            entity.ToTable("Location");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Draft")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Review_pk");

            entity.ToTable("Review");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("review_date");
            entity.Property(e => e.TourId).HasColumnName("tour_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Tour).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Review_Tour");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Review_User");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tour_pk");

            entity.ToTable("Tour");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("price");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Draft")
                .HasColumnName("status");
        });

        modelBuilder.Entity<TourInstance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tour_Instance_pk");

            entity.ToTable("Tour_Instance");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookedSlots).HasColumnName("booked_slots");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Open")
                .HasColumnName("status");
            entity.Property(e => e.TourId).HasColumnName("tour_id");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourInstances)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Tour_Instance_Tour");
        });

        modelBuilder.Entity<TourLocation>(entity =>
        {
            entity.HasKey(e => new { e.LocationId, e.TourId }).HasName("Tour_Location_pk");

            entity.ToTable("Tour_Location");

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.TourId).HasColumnName("tour_id");
            entity.Property(e => e.OrderInTour).HasColumnName("order_in_tour");

            entity.HasOne(d => d.Location).WithMany(p => p.TourLocations)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Tour_Location_Location");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourLocations)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Tour_Location_Tour");
        });

    }
}
