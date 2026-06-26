namespace CRM.Infrastructure.Configuration;

public class SeedDataSettings
{
    public const string SectionName = "SeedData";

    public string AdminEmail { get; set; } = string.Empty;

    public string AdminPassword { get; set; } = string.Empty;
}