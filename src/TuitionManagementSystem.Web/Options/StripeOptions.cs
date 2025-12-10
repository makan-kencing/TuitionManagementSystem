namespace TuitionManagementSystem.Web.Options;

using System.Configuration;

public class StripeOptions : ConfigurationSection
{
    [StringValidator(MinLength = 1)]
    public required string PublishableKey { get; init; }

    [StringValidator(MinLength = 1)]
    public required string SecretKey { get; init; }
}
