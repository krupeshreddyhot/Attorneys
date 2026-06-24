using Attorneys.Application.Common.Interfaces;
using Attorneys.Infrastructure.Common;
using Attorneys.Infrastructure.Persistence;
using Attorneys.Infrastructure.Services;
using Attorneys.Infrastructure.Services.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Attorneys.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IDocumentTextExtractionService, DocumentTextExtractionService>();
        services.AddScoped<IOpenAIService, OpenAIService>();
        services.AddScoped<IDocumentAnalysisService, DocumentAnalysisService>();
        services.AddSingleton<IDocumentStorageService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var provider = DocumentSettingsResolver.Get(config, "Documents:Provider") ?? "local";
            return provider.Equals("s3", StringComparison.OrdinalIgnoreCase)
                ? new S3DocumentStorageService(config, sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<S3DocumentStorageService>>())
                : new LocalDocumentStorageService(config);
        });

        return services;
    }
}
