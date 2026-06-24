using System.Reflection;
using Attorneys.Application.Common.Interfaces;
using Attorneys.Domain.Common;
using Attorneys.Domain.Entities;
using Attorneys.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Tenant> TenantSet => Set<Tenant>();
    public DbSet<User> UserSet => Set<User>();
    public DbSet<Court> CourtSet => Set<Court>();
    public DbSet<CaseType> CaseTypeSet => Set<CaseType>();
    public DbSet<LegalCase> CaseSet => Set<LegalCase>();
    public DbSet<CaseDetail> CaseDetailSet => Set<CaseDetail>();
    public DbSet<CaseAccount> CaseAccountSet => Set<CaseAccount>();
    public DbSet<CasePayment> CasePaymentSet => Set<CasePayment>();
    public DbSet<CaseDocument> CaseDocumentSet => Set<CaseDocument>();
    public DbSet<CaseDocumentAIAnalysis> CaseDocumentAIAnalysisSet => Set<CaseDocumentAIAnalysis>();
    public DbSet<TenantBannerImage> TenantBannerImageSet => Set<TenantBannerImage>();
    public DbSet<TenantPracticeArea> TenantPracticeAreaSet => Set<TenantPracticeArea>();
    public DbSet<TenantAdvocate> TenantAdvocateSet => Set<TenantAdvocate>();

    public IQueryable<Tenant> Tenants => TenantSet.AsQueryable();
    public IQueryable<User> Users => UserSet.AsQueryable();
    public IQueryable<Court> Courts => CourtSet.AsQueryable();
    public IQueryable<CaseType> CaseTypes => CaseTypeSet.AsQueryable();
    public IQueryable<LegalCase> Cases => CaseSet.AsQueryable();
    public IQueryable<CaseDetail> CaseDetails => CaseDetailSet.AsQueryable();
    public IQueryable<CaseAccount> CaseAccounts => CaseAccountSet.AsQueryable();
    public IQueryable<CasePayment> CasePayments => CasePaymentSet.AsQueryable();
    public IQueryable<CaseDocument> CaseDocuments => CaseDocumentSet.AsQueryable();
    public IQueryable<CaseDocumentAIAnalysis> CaseDocumentAIAnalyses => CaseDocumentAIAnalysisSet.AsQueryable();
    public IQueryable<TenantBannerImage> TenantBannerImages => TenantBannerImageSet.AsQueryable();
    public IQueryable<TenantPracticeArea> TenantPracticeAreas => TenantPracticeAreaSet.AsQueryable();
    public IQueryable<TenantAdvocate> TenantAdvocates => TenantAdvocateSet.AsQueryable();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tenant>(e =>
        {
            e.ToTable("tenants");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.AddressLine).HasMaxLength(300);
            e.Property(x => x.City).HasMaxLength(100);
            e.Property(x => x.Phone).HasMaxLength(50);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.HeroTagline).HasMaxLength(300);
            e.Property(x => x.HeroSubtitle).HasMaxLength(500);
            e.Property(x => x.AboutTitle).HasMaxLength(300);
            e.Property(x => x.AboutBody).HasMaxLength(4000);
            e.Property(x => x.AboutHighlightTitle).HasMaxLength(200);
            e.Property(x => x.AboutHighlightBody).HasMaxLength(2000);
        });

        builder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TenantId, x.UserName }).IsUnique();
            e.Property(x => x.UserName).HasMaxLength(50);
        });

        builder.Entity<Court>(e =>
        {
            e.ToTable("courts");
            e.HasKey(x => new { x.TenantId, x.CourtId });
            e.Property(x => x.CourtId).HasMaxLength(20);
            e.Property(x => x.CourtName).HasMaxLength(200).IsRequired();
            e.Property(x => x.CourtCity).HasMaxLength(100);
        });

        builder.Entity<CaseType>(e =>
        {
            e.ToTable("case_types");
            e.HasKey(x => new { x.TenantId, x.CaseTypeId });
            e.Property(x => x.CaseTypeId).HasMaxLength(20);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        builder.Entity<LegalCase>(e =>
        {
            e.ToTable("cases");
            e.HasKey(x => new { x.TenantId, x.CaseNo });
            e.Property(x => x.CaseNo).HasMaxLength(50);
            e.HasOne(x => x.Court).WithMany(c => c.Cases)
                .HasForeignKey(x => new { x.TenantId, x.CourtId });
            e.HasOne(x => x.CaseType).WithMany(t => t.Cases)
                .HasForeignKey(x => new { x.TenantId, x.CaseTypeId });
        });

        builder.Entity<CaseDetail>(e =>
        {
            e.ToTable("case_details");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TenantId, x.CaseNo, x.CaseNoId }).IsUnique();
            e.HasOne(x => x.Case).WithMany(c => c.Details)
                .HasForeignKey(x => new { x.TenantId, x.CaseNo });
        });

        builder.Entity<CaseAccount>(e =>
        {
            e.ToTable("case_accounts");
            e.HasKey(x => new { x.TenantId, x.CaseNo });
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            e.HasOne(x => x.Case).WithOne(c => c.Account)
                .HasForeignKey<CaseAccount>(x => new { x.TenantId, x.CaseNo });
        });

        builder.Entity<CasePayment>(e =>
        {
            e.ToTable("case_payments");
            e.HasKey(x => x.Id);
            e.Property(x => x.AmountPaid).HasPrecision(18, 2);
            e.HasOne(x => x.Account).WithMany(a => a.Payments)
                .HasForeignKey(x => new { x.TenantId, x.CaseNo });
        });

        builder.Entity<CaseDocument>(e =>
        {
            e.ToTable("case_documents");
            e.HasKey(x => x.FileId);
            e.Property(x => x.CaseNo).HasMaxLength(50);
            e.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.FileType).HasMaxLength(50);
            e.Property(x => x.StorageKey).HasMaxLength(500).IsRequired();
            e.HasOne(x => x.Case).WithMany()
                .HasForeignKey(x => new { x.TenantId, x.CaseNo });
        });

        builder.Entity<CaseDocumentAIAnalysis>(e =>
        {
            e.ToTable("case_document_ai_analysis");
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<int>();
            e.Property(x => x.Summary).HasColumnType("text");
            e.Property(x => x.KeyPointsJson).HasColumnType("jsonb");
            e.Property(x => x.PartiesJson).HasColumnType("jsonb");
            e.Property(x => x.ImportantDatesJson).HasColumnType("jsonb");
            e.Property(x => x.NextActionsJson).HasColumnType("jsonb");
            e.Property(x => x.AIModel).HasMaxLength(100);
            e.Property(x => x.PromptVersion).HasMaxLength(20);
            e.Property(x => x.EstimatedCost).HasPrecision(18, 6);
            e.Property(x => x.ExtractedTextStorageKey).HasMaxLength(500);
            e.Property(x => x.ErrorMessage).HasMaxLength(2000);
            e.HasIndex(x => x.TenantId);
            e.HasIndex(x => x.FileId)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");
            e.HasOne(x => x.Document).WithMany()
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TenantBannerImage>(e =>
        {
            e.ToTable("tenant_banner_images");
            e.HasKey(x => x.Id);
            e.Property(x => x.StorageKey).HasMaxLength(500).IsRequired();
            e.Property(x => x.Caption).HasMaxLength(300);
        });

        builder.Entity<TenantPracticeArea>(e =>
        {
            e.ToTable("tenant_practice_areas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
        });

        builder.Entity<TenantAdvocate>(e =>
        {
            e.ToTable("tenant_advocates");
            e.HasKey(x => x.Id);
            e.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            e.Property(x => x.Designation).HasMaxLength(150);
            e.Property(x => x.PhotoStorageKey).HasMaxLength(500);
            e.Property(x => x.Bio).HasMaxLength(2000);
        });

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { builder });
            }
        }
    }

    private void SetTenantFilter<TEntity>(ModelBuilder builder) where TEntity : class, ITenantEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e =>
            !e.IsDeleted &&
            (_currentUserService == null
             || IsSuperAdminCrossTenant()
             || e.TenantId == _currentUserService.TenantId));
    }

    private bool IsSuperAdminCrossTenant() =>
        _currentUserService != null
        && string.Equals(_currentUserService.Role, nameof(UserRole.SuperAdmin), StringComparison.OrdinalIgnoreCase);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService?.TenantId ?? 0;

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TenantId == 0 && tenantId > 0)
                entry.Entity.TenantId = tenantId;
        }

        foreach (var entry in ChangeTracker.Entries<LegalCase>().Where(e => e.State == EntityState.Added))
        {
            var caseTenantId = entry.Entity.TenantId;
            foreach (var detail in entry.Entity.Details.Where(d => d.TenantId == 0))
                detail.TenantId = caseTenantId;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
