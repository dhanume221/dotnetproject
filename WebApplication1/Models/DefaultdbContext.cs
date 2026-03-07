using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class EmployeeContext : DbContext
{
    public EmployeeContext()
    {
    }

    public EmployeeContext(DbContextOptions<EmployeeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Userdatum> Userdata { get; set; }
    public virtual DbSet<Foodlist> Foodlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=pg-dhanushdb-dhanushmr88-c3fd.d.aivencloud.com;Port=20768;Database=defaultdb;Username=avnadmin;Password=AVNS_BD_4R7V2nlf4PaaAKwi");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.Entity<Employee>(entity =>
        {
            
            entity.HasKey(e => e.Id).HasName("pk_id");

            entity.ToTable("employees");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Foodlist>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("foodlist");

            entity.Property(e => e.Foodname)
                .HasMaxLength(20)
                .HasColumnName("foodname");
            entity.Property(e => e.Menuid)
                .HasPrecision(10)
                .HasColumnName("menuid");
            entity.Property(e => e.Price)
                .HasPrecision(5, 2)
                .HasColumnName("price");
        });

        modelBuilder.Entity<Userdatum>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("userdata_pkey");

            entity.ToTable("userdata");

            entity.Property(e => e.Userid)
                .HasPrecision(10)
                .HasColumnName("userid");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasPrecision(10)
                .HasColumnName("phone");
            entity.Property(e => e.Username)
                .HasMaxLength(10)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
