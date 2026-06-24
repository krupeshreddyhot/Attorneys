using Attorneys.Domain.Entities;

namespace Attorneys.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, string? firmCode = null, string? firmName = null);
}
