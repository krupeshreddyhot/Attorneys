namespace Attorneys.Domain.Entities;

public class Tenant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? HeroTagline { get; set; }
    public string? HeroSubtitle { get; set; }
    public string? AboutTitle { get; set; }
    public string? AboutBody { get; set; }
    public string? AboutHighlightTitle { get; set; }
    public string? AboutHighlightBody { get; set; }
}
