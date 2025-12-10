namespace TuitionManagementSystem.Web.Configurations;

using System.Configuration;

public class LuckyPennySoftwareOptions : ConfigurationSection
{
    public required string LicenseKey { get; init; }
}
