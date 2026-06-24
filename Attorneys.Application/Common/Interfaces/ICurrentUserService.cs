namespace Attorneys.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    string Role { get; }
    int TenantId { get; }
}
