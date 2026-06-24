using Attorneys.Domain.Entities;
using Attorneys.Domain.Enums;
using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attorneys.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext db,
        ILogger logger,
        bool isDevelopment = false,
        CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);

        var demoTenant = await db.Tenants.FirstOrDefaultAsync(t => t.Code == "DEMO", cancellationToken);
        if (demoTenant is null)
        {
            demoTenant = new Tenant
            {
                Name = "Demo Law Firm",
                Code = "DEMO",
                IsActive = true
            };
            db.TenantSet.Add(demoTenant);
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded default tenant: DEMO (Demo Law Firm)");
        }

        var hasher = new PasswordHasher<User>();

        if (!await db.Users.IgnoreQueryFilters().AnyAsync(
                u => !u.IsDeleted && u.Role == UserRole.SuperAdmin,
                cancellationToken))
        {
            var superAdmin = new User
            {
                TenantId = 0,
                UserName = "superadmin",
                PasswordHash = string.Empty,
                Role = UserRole.SuperAdmin,
                IsActive = true
            };
            superAdmin.PasswordHash = hasher.HashPassword(superAdmin, "SuperAdmin@123");
            db.UserSet.Add(superAdmin);
            logger.LogInformation("Seeded SuperAdmin: superadmin / SuperAdmin@123");
        }

        var admin = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => !u.IsDeleted && u.UserName.ToLower() == "admin", cancellationToken);
        if (admin is null)
        {
            admin = new User
            {
                TenantId = demoTenant.Id,
                UserName = "admin",
                PasswordHash = string.Empty,
                Role = UserRole.Administrator,
                IsActive = true
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
            db.UserSet.Add(admin);
            logger.LogInformation("Seeded admin / Admin@123 for firm DEMO");
        }
        else if (admin.TenantId != demoTenant.Id)
        {
            admin.TenantId = demoTenant.Id;
            logger.LogInformation("Updated admin user to tenant DEMO");
        }

        if (!await db.Users.IgnoreQueryFilters().AnyAsync(
                u => !u.IsDeleted && u.UserName.ToLower() == "staff",
                cancellationToken))
        {
            var general = new User
            {
                TenantId = demoTenant.Id,
                UserName = "staff",
                PasswordHash = string.Empty,
                Role = UserRole.General,
                IsActive = true
            };
            general.PasswordHash = hasher.HashPassword(general, "Staff@123");
            db.UserSet.Add(general);
            logger.LogInformation("Seeded staff / Staff@123 for firm DEMO");
        }

        if (!await db.Courts.IgnoreQueryFilters().AnyAsync(c => c.TenantId == demoTenant.Id, cancellationToken))
        {
            db.CourtSet.AddRange(
                new Court { TenantId = demoTenant.Id, CourtId = "1", CourtName = "Sessions Court", CourtCity = "Nizamabad" },
                new Court { TenantId = demoTenant.Id, CourtId = "2", CourtName = "District Court", CourtCity = "Hyderabad" },
                new Court { TenantId = demoTenant.Id, CourtId = "3", CourtName = "High Court", CourtCity = "Hyderabad" });
        }

        if (!await db.CaseTypes.IgnoreQueryFilters().AnyAsync(t => t.TenantId == demoTenant.Id, cancellationToken))
        {
            db.CaseTypeSet.AddRange(
                new CaseType { TenantId = demoTenant.Id, CaseTypeId = "1", Name = "Criminal" },
                new CaseType { TenantId = demoTenant.Id, CaseTypeId = "2", Name = "Civil" },
                new CaseType { TenantId = demoTenant.Id, CaseTypeId = "3", Name = "Family" },
                new CaseType { TenantId = demoTenant.Id, CaseTypeId = "4", Name = "Bail" });
        }

        await db.SaveChangesAsync(cancellationToken);

        await SeedDemoWebsiteContentAsync(db, demoTenant, logger, cancellationToken);

        if (isDevelopment)
            await ResetDevPasswordsAsync(db, demoTenant.Id, logger, cancellationToken);
    }

    private static async Task ResetDevPasswordsAsync(
        ApplicationDbContext db,
        int demoTenantId,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var hasher = new PasswordHasher<User>();
        var accounts = new (string UserName, string Password, UserRole Role, int TenantId)[]
        {
            ("superadmin", "SuperAdmin@123", UserRole.SuperAdmin, 0),
            ("admin", "Admin@123", UserRole.Administrator, demoTenantId),
            ("staff", "Staff@123", UserRole.General, demoTenantId),
        };

        foreach (var (userName, password, role, tenantId) in accounts)
        {
            var user = await db.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.UserName.ToLower() == userName, cancellationToken);
            if (user is null) continue;

            user.Role = role;
            user.TenantId = tenantId;
            user.IsActive = true;
            user.PasswordHash = hasher.HashPassword(user, password);
            logger.LogInformation("Development: reset password for {UserName}", userName);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoWebsiteContentAsync(
        ApplicationDbContext db,
        Tenant demoTenant,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(demoTenant.HeroTagline))
        {
            demoTenant.AddressLine = "Nizamabad · Hyderabad";
            demoTenant.City = "Telangana";
            demoTenant.Phone = "+91 98765 43210";
            demoTenant.Email = "chamber@demo-attorneys.local";
            demoTenant.HeroTagline = "We provide legal help for your cases";
            demoTenant.HeroSubtitle = "Trusted chamber for court case management, diary tracking, and client representation across criminal and civil matters.";
            demoTenant.AboutTitle = "Experienced advocates for courts in Telangana";
            demoTenant.AboutBody = "Our chamber supports advocates with structured case records, hearing dates, court-wise organization, and accounts — the same workflow trusted for years, now available on the web.";
            demoTenant.AboutHighlightTitle = "Experience · Trust · Results";
            demoTenant.AboutHighlightBody = "From case filing to next-date diary and payment tracking, manage your practice in one secure application.";
            logger.LogInformation("Seeded DEMO tenant website profile content");
        }

        if (!await db.TenantPracticeAreas.IgnoreQueryFilters().AnyAsync(p => p.TenantId == demoTenant.Id, cancellationToken))
        {
            db.TenantPracticeAreaSet.AddRange(
                new TenantPracticeArea { TenantId = demoTenant.Id, Title = "Criminal Law", Description = "Defense and prosecution support across sessions and district courts.", SortOrder = 1 },
                new TenantPracticeArea { TenantId = demoTenant.Id, Title = "Civil Litigation", Description = "Property, contract, and recovery matters.", SortOrder = 2 },
                new TenantPracticeArea { TenantId = demoTenant.Id, Title = "Family Law", Description = "Matrimonial, custody, and maintenance cases.", SortOrder = 3 },
                new TenantPracticeArea { TenantId = demoTenant.Id, Title = "Bail & Trial", Description = "Urgent bail applications and trial representation.", SortOrder = 4 });
            logger.LogInformation("Seeded DEMO tenant practice areas");
        }

        if (!await db.TenantAdvocates.IgnoreQueryFilters().AnyAsync(a => a.TenantId == demoTenant.Id, cancellationToken))
        {
            db.TenantAdvocateSet.AddRange(
                new TenantAdvocate
                {
                    TenantId = demoTenant.Id,
                    FullName = "Adv. Ramesh Kumar",
                    Designation = "Senior Advocate",
                    Bio = "Over 20 years of experience in criminal and civil litigation across Telangana courts.",
                    SortOrder = 1,
                },
                new TenantAdvocate
                {
                    TenantId = demoTenant.Id,
                    FullName = "Adv. Priya Sharma",
                    Designation = "Associate Advocate",
                    Bio = "Specializes in family law, bail matters, and client counseling.",
                    SortOrder = 2,
                });
            logger.LogInformation("Seeded DEMO tenant advocates");
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
