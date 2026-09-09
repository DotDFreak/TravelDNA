namespace TravelDNA.Core.Models;

public sealed class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public string? GoogleSubjectId { get; set; }
    public required string AuthProvider { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
