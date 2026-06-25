using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}