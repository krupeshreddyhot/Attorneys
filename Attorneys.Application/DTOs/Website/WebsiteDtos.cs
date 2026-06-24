namespace Attorneys.Application.DTOs.Website;

public record FirmLandingDto(
    string Code,
    string Name,
    string? AddressLine,
    string? City,
    string? Phone,
    string? Email,
    string? HeroTagline,
    string? HeroSubtitle,
    string? AboutTitle,
    string? AboutBody,
    string? AboutHighlightTitle,
    string? AboutHighlightBody,
    IReadOnlyList<BannerDto> Banners,
    IReadOnlyList<PracticeAreaDto> PracticeAreas,
    IReadOnlyList<AdvocateDto> Advocates);

public record BannerDto(int Id, string? Caption, int SortOrder, string ImageUrl);

public record PracticeAreaDto(int Id, string Title, string? Description, int SortOrder);

public record AdvocateDto(int Id, string FullName, string? Designation, string? Bio, int SortOrder, string? PhotoUrl);

public record WebsiteProfileDto(
    string Name,
    string Code,
    string? AddressLine,
    string? City,
    string? Phone,
    string? Email,
    string? HeroTagline,
    string? HeroSubtitle,
    string? AboutTitle,
    string? AboutBody,
    string? AboutHighlightTitle,
    string? AboutHighlightBody);

public record UpdateWebsiteProfileRequest(
    string? AddressLine,
    string? City,
    string? Phone,
    string? Email,
    string? HeroTagline,
    string? HeroSubtitle,
    string? AboutTitle,
    string? AboutBody,
    string? AboutHighlightTitle,
    string? AboutHighlightBody);

public record UpsertPracticeAreaRequest(string Title, string? Description, int SortOrder);

public record UpsertAdvocateRequest(string FullName, string? Designation, string? Bio, int SortOrder);

public record UpdateBannerRequest(string? Caption, int SortOrder);
