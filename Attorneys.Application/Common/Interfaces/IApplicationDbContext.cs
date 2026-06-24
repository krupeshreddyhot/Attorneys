using Attorneys.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<Tenant> Tenants { get; }

    IQueryable<User> Users { get; }

    IQueryable<Court> Courts { get; }

    IQueryable<CaseType> CaseTypes { get; }

    IQueryable<LegalCase> Cases { get; }

    IQueryable<CaseDetail> CaseDetails { get; }

    IQueryable<CaseAccount> CaseAccounts { get; }

    IQueryable<CasePayment> CasePayments { get; }

    IQueryable<CaseDocument> CaseDocuments { get; }

    IQueryable<CaseDocumentAIAnalysis> CaseDocumentAIAnalyses { get; }

    DbSet<CaseDocumentAIAnalysis> CaseDocumentAIAnalysisSet { get; }

    IQueryable<TenantBannerImage> TenantBannerImages { get; }

    IQueryable<TenantPracticeArea> TenantPracticeAreas { get; }

    IQueryable<TenantAdvocate> TenantAdvocates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

