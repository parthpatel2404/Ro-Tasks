using System;
using System.Collections.Generic;
using IDVerification.Models;
using Microsoft.EntityFrameworkCore;

namespace IDVerification.Data;

public partial class IDVDbContext : DbContext
{
    public IDVDbContext()
    {
    }

    public IDVDbContext(DbContextOptions<IDVDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FatfJurisdictionRating> FatfJurisdictionRatings { get; set; }

    public virtual DbSet<UserTable> UserTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=host.docker.internal;port=3306;user=root;password=Tatva@123;database=rodatabasetask");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FatfJurisdictionRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fatf_jurisdiction_ratings");

            entity.Property(e => e.CountryEffectiveRating).HasMaxLength(25);
            entity.Property(e => e.EffectiveScore).HasPrecision(10);
            entity.Property(e => e.Jurisdiction).HasMaxLength(60);
            entity.Property(e => e.TechnicalComplianceRating).HasMaxLength(25);
            entity.Property(e => e.TechnicalComplianceScore).HasPrecision(10);
        });

        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user_table");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(45)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(45)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(45)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
