using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using pharmace.Models;

namespace pharmace;

public partial class PharmacyContext : DbContext
{
    public PharmacyContext()
    {
    }

    public PharmacyContext(DbContextOptions<PharmacyContext> options)
        : base(options)
    {
    }
    public virtual DbSet<userpermations> userpermations { get; set; }
    public virtual DbSet<orderprodect> orderprodects { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Orders> Orders { get; set; }
    public virtual DbSet<Proudect> Proudects { get; set; }
    public virtual DbSet<Offer> Offers { get; set; }
    public DbSet<Notification> Notifications { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-PC\\SQLEXPRESS;Database=xxx4;Encrypt=false;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>()
    .HasKey(e => new { e.proudectId, e.userId }); 
        modelBuilder.Entity<orderprodect>()
    .HasKey(e => new { e.proudectId, e.orderId });

    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
