using DocumentStorage.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data;

public partial class DocumentStorageDbContext : DbContext
{
    public DocumentStorageDbContext()
    {
    }

    public DocumentStorageDbContext(DbContextOptions<DocumentStorageDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountUser> AccountUsers { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<PlanLimit> PlanLimits { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<UsageRecord> UsageRecords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=DocumentStorageDB;User Id=sa;Password=reallyStrongPwd123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accounts__3214EC07EBA36FBD");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AccountType).HasDefaultValue("User");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<AccountUser>(entity =>
        {
            entity.Property(e => e.Role).HasDefaultValue("Owner");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountUsers_Accounts");

            entity.HasOne(d => d.User).WithMany(p => p.AccountUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountUsers_Users");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3214EC076AA7FA7E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Account).WithMany(p => p.Documents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Documents_Accounts");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Plans__3214EC076ADE053E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BillingCycle).HasDefaultValue("Monthly");
            entity.Property(e => e.Currency).HasDefaultValue("EUR");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<PlanLimit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlanLimi__3214EC074B6499BB");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsHardLimit).HasDefaultValue(true);

            entity.HasOne(d => d.Plan).WithMany(p => p.PlanLimits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlanLimits_Plans");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07A9C2DC9A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.PeriodEnd).HasDefaultValueSql("(dateadd(month,(1),getdate()))");
            entity.Property(e => e.PeriodStart).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Active");

            entity.HasOne(d => d.Account).WithMany(p => p.Subscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriptions_Accounts");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriptions_Plans");
        });

        modelBuilder.Entity<UsageRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsageRec__3214EC07ADE860E4");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.PeriodEnd).HasDefaultValueSql("(dateadd(month,(1),getdate()))");
            entity.Property(e => e.PeriodStart).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Subscription).WithMany(p => p.UsageRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsageRecords_Subscriptions");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0791B951F9");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
