using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;
[Obsolete("DO NOT USE - DEPRECATED")]
public partial class GlassesOrderDatabaseContext : DbContext
{
    public GlassesOrderDatabaseContext()
    {
    }

    public GlassesOrderDatabaseContext(DbContextOptions options)
        : base(options)
    {
    }

    public virtual DbSet<GlassesOrder> GlassesOrders { get; set; }

    #region OLD OPTIONS CLASSES (UNUSABLE ?)
    public virtual DbSet<Options9> Options9s { get; set; }

    public virtual DbSet<Options94> Options94s { get; set; }

    public virtual DbSet<Options9A> Options9As { get; set; }

    public virtual DbSet<Options9B> Options9Bs { get; set; }

    public virtual DbSet<Options9F> Options9Fs { get; set; }

    public virtual DbSet<OptionsDb> OptionsDbs { get; set; }

    public virtual DbSet<OptionsE> OptionsEs { get; set; }

    public virtual DbSet<OptionsHb> OptionsHbs { get; set; }

    public virtual DbSet<OptionsNb> OptionsNbs { get; set; }

    public virtual DbSet<OptionsNp> OptionsNps { get; set; }

    public virtual DbSet<OptionsV> OptionsVs { get; set; }

    public virtual DbSet<OptionsV4> OptionsV4s { get; set; }

    public virtual DbSet<OptionsVa> OptionsVas { get; set; }

    public virtual DbSet<OptionsVf> OptionsVfs { get; set; }

    public virtual DbSet<OptionsW> OptionsWs { get; set; }

    public virtual DbSet<OptionsW1> OptionsWs1 { get; set; } 
    #endregion

    /// <summary>
    /// Will only as as A Code Container for the Glasses that are already made
    /// </summary>
    public virtual DbSet<OrderedCabin> OrderedCabins { get; set; }
    /// <summary>
    /// These are the only ones that Matter
    /// </summary>
    public virtual DbSet<OrderedGlass> OrderedGlasses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=GlassesOrderDatabase.db;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GlassesOrder>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_GlassesOrders_Id").IsUnique();

            entity.HasIndex(e => e.SupplierOrderNo, "IX_GlassesOrders_SupplierOrderNo").IsUnique();
        });

        modelBuilder.Entity<Options9>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Options9S");

            entity.HasIndex(e => e.OrderedCabinId, "IX_Options9S_OrderedCabinId").IsUnique();

            entity.Property(e => e.CoverDistanceCd).HasColumnName("CoverDistanceCD");
            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.HandleDistanceHd).HasColumnName("HandleDistanceHD");
            entity.Property(e => e.MagnetAluminiumThickAl3).HasColumnName("MagnetAluminiumThickAL3");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.OverlapEpik).HasColumnName("OverlapEPIK");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");
            entity.Property(e => e.WallAluminiumAl2).HasColumnName("WallAluminiumAL2");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<Options9>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<Options94>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Options94");

            entity.HasIndex(e => e.OrderedCabinId, "IX_Options94_OrderedCabinId").IsUnique();

            entity.Property(e => e.CoverDistanceCd).HasColumnName("CoverDistanceCD");
            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.HandleDistanceHd).HasColumnName("HandleDistanceHD");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.OverlapEpik).HasColumnName("OverlapEPIK");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");
            entity.Property(e => e.WallAluminiumAl2).HasColumnName("WallAluminiumAL2");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<Options94>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<Options9A>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Options9A");

            entity.HasIndex(e => e.OrderedCabinId, "IX_Options9A_OrderedCabinId").IsUnique();

            entity.Property(e => e.CoverDistanceCd).HasColumnName("CoverDistanceCD");
            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.HandleDistanceHd).HasColumnName("HandleDistanceHD");
            entity.Property(e => e.L0type).HasColumnName("L0Type");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.OverlapEpik).HasColumnName("OverlapEPIK");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<Options9A>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<Options9B>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Options9B");

            entity.HasIndex(e => e.OrderedCabinId, "IX_Options9B_OrderedCabinId").IsUnique();

            entity.Property(e => e.GlassGapAer).HasColumnName("GlassGapAER");
            entity.Property(e => e.MagnetAluminiumThickAl3).HasColumnName("MagnetAluminiumThickAL3");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");
            entity.Property(e => e.WallAluminiumAl2).HasColumnName("WallAluminiumAL2");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<Options9B>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<Options9F>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Options9F");

            entity.HasIndex(e => e.OrderedCabinId, "IX_Options9F_OrderedCabinId").IsUnique();

            entity.Property(e => e.ConnectorAluminiumAlc).HasColumnName("ConnectorAluminiumALC");
            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<Options9F>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsDb>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsDB");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsDB_OrderedCabinId").IsUnique();

            entity.Property(e => e.GlassGapAer).HasColumnName("GlassGapAER");
            entity.Property(e => e.MagnetAluminiumAlmag).HasColumnName("MagnetAluminiumALMAG");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsDb>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsE>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsE");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsE_OrderedCabinId").IsUnique();

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsE>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsHb>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsHB");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsHB_OrderedCabinId").IsUnique();

            entity.Property(e => e.GlassGapAer).HasColumnName("GlassGapAER");
            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.MagnetAluminiumAlmag).HasColumnName("MagnetAluminiumALMAG");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsHb>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsNb>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsNB");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsNB_OrderedCabinId").IsUnique();

            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.MagnetAluminiumAlmag).HasColumnName("MagnetAluminiumALMAG");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsNb>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsNp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsNP");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsNP_OrderedCabinId").IsUnique();

            entity.Property(e => e.ConnectorAluminiumAlc).HasColumnName("ConnectorAluminiumALC");
            entity.Property(e => e.GlassGapAer).HasColumnName("GlassGapAER");
            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.MagnetAluminiumAlmag).HasColumnName("MagnetAluminiumALMAG");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsNp>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsV>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsVS");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsVS_OrderedCabinId").IsUnique();

            entity.Property(e => e.CoverDistanceCd).HasColumnName("CoverDistanceCD");
            entity.Property(e => e.HandleDistanceHd).HasColumnName("HandleDistanceHD");
            entity.Property(e => e.MagnetAluminiumAlmag).HasColumnName("MagnetAluminiumALMAG");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.OverlapEpik).HasColumnName("OverlapEPIK");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsV>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsV4>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsV4");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsV4_OrderedCabinId").IsUnique();

            entity.Property(e => e.CoverDistanceCd).HasColumnName("CoverDistanceCD");
            entity.Property(e => e.HandleDistanceHd).HasColumnName("HandleDistanceHD");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.OverlapEpik).HasColumnName("OverlapEPIK");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsV4>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsVa>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsVA");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsVA_OrderedCabinId").IsUnique();

            entity.Property(e => e.CoverDistanceCd).HasColumnName("CoverDistanceCD");
            entity.Property(e => e.HandleDistanceHd).HasColumnName("HandleDistanceHD");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.OverlapEpik).HasColumnName("OverlapEPIK");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsVa>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsVf>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsVF");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsVF_OrderedCabinId").IsUnique();

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsVf>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsW>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsW");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsW_OrderedCabinId").IsUnique();

            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsW>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OptionsW1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OptionsWS");

            entity.HasIndex(e => e.OrderedCabinId, "IX_OptionsWS_OrderedCabinId").IsUnique();

            entity.Property(e => e.CoverDistanceCd).HasColumnName("CoverDistanceCD");
            entity.Property(e => e.GlassInAluminiumAlst).HasColumnName("GlassInAluminiumALST");
            entity.Property(e => e.HandleDistanceHd).HasColumnName("HandleDistanceHD");
            entity.Property(e => e.MagnetAluminiumAlmag).HasColumnName("MagnetAluminiumALMAG");
            entity.Property(e => e.MagnetStripMagn).HasColumnName("MagnetStripMAGN");
            entity.Property(e => e.OverlapEpik).HasColumnName("OverlapEPIK");
            entity.Property(e => e.WallAluminiumAl1).HasColumnName("WallAluminiumAL1");

            entity.HasOne(d => d.OrderedCabin).WithOne().HasForeignKey<OptionsW1>(d => d.OrderedCabinId);
        });

        modelBuilder.Entity<OrderedCabin>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_OrderedCabins_Id").IsUnique();

            entity.HasOne(d => d.GlassesOrder).WithMany(p => p.OrderedCabins).HasForeignKey(d => d.GlassesOrderId);
        });

        modelBuilder.Entity<OrderedGlass>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_OrderedGlasses_Id").IsUnique();

            entity.Property(e => e.Cost).HasColumnType("NUMERIC");

            entity.HasOne(d => d.GlassesOrder).WithMany(p => p.OrderedGlasses).HasForeignKey(d => d.GlassesOrderId);

            entity.HasOne(d => d.OrderedCabin).WithMany(p => p.OrderedGlasses).HasForeignKey(d => d.OrderedCabinId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
