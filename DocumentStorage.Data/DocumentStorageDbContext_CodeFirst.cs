using Microsoft.EntityFrameworkCore;
using DocumentStorage.Data.Models;

namespace DocumentStorage.Data
{
    public class DocumentStorageDbContextCodeFirst : DbContext
    {
        public DocumentStorageDbContextCodeFirst(DbContextOptions<DocumentStorageDbContextCodeFirst> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<AccountUser> AccountUsers => Set<AccountUser>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<Plan> Plans => Set<Plan>();
        public DbSet<PlanLimit> PlanLimits => Set<PlanLimit>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<UsageRecord> UsageRecords => Set<UsageRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.UseIdentityColumns();

            // --------------------
            // Account
            // --------------------
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccountType)
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasDefaultValue("User");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });

            // --------------------
            // User
            // --------------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // --------------------
            // AccountUser (many-to-many)
            // --------------------
            modelBuilder.Entity<AccountUser>(entity =>
            {
                entity.ToTable("AccountUsers");

                entity.HasKey(e => new { e.AccountId, e.UserId });

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasDefaultValue("Owner");

                entity.HasOne(e => e.Account)
                    .WithMany(a => a.AccountUsers)
                    .HasForeignKey(e => e.AccountId)
                    .HasConstraintName("FK_AccountUsers_Accounts");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.AccountUsers)
                    .HasForeignKey(e => e.UserId)
                    .HasConstraintName("FK_AccountUsers_Users");
            });

            // --------------------
            // Document
            // --------------------
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Documents");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.ContentType)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.HasOne(e => e.Account)
                    .WithMany(a => a.Documents)
                    .HasForeignKey(e => e.AccountId)
                    .HasConstraintName("FK_Documents_Accounts");
            });

            // --------------------
            // Plan
            // --------------------
            modelBuilder.Entity<Plan>(entity =>
            {
                entity.ToTable("Plans");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .HasDefaultValue("EUR");

                entity.Property(e => e.BillingCycle)
                    .HasMaxLength(50)
                    .HasDefaultValue("Monthly");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.Code)
                    .IsUnique();
            });

            // --------------------
            // PlanLimit
            // --------------------
            modelBuilder.Entity<PlanLimit>(entity =>
            {
                entity.ToTable("PlanLimits");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ResourceType)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.IsHardLimit)
                    .HasDefaultValue(true);

                entity.HasOne(e => e.Plan)
                    .WithMany(p => p.PlanLimits)
                    .HasForeignKey(e => e.PlanId)
                    .HasConstraintName("FK_PlanLimits_Plans");
            });

            // --------------------
            // Subscription
            // --------------------
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.ToTable("Subscriptions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Active");

                entity.Property(e => e.PeriodStart)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PeriodEnd)
                    .HasDefaultValueSql("(dateadd(month,(1),getdate()))");

                entity.HasOne(e => e.Account)
                    .WithMany(a => a.Subscriptions)
                    .HasForeignKey(e => e.AccountId)
                    .HasConstraintName("FK_Subscriptions_Accounts");

                entity.HasOne(e => e.Plan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(e => e.PlanId)
                    .HasConstraintName("FK_Subscriptions_Plans");
            });

            // --------------------
            // UsageRecord
            // --------------------
            modelBuilder.Entity<UsageRecord>(entity =>
            {
                entity.ToTable("UsageRecords");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ResourceType)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.PeriodStart)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PeriodEnd)
                    .HasDefaultValueSql("(dateadd(month,(1),getdate()))");

                entity.HasOne(e => e.Subscription)
                    .WithMany(s => s.UsageRecords)
                    .HasForeignKey(e => e.SubscriptionId)
                    .HasConstraintName("FK_UsageRecords_Subscriptions");
            });
        }
    }
}