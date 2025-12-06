namespace TuitionManagementSystem.Web.Configurations;

using System.Configuration;
using System.Net;
using System.Net.Mail;

public class SmtpOptions : ConfigurationSection
{
    [StringValidator(MinLength = 1)]
    public required string Host { get; init; }

    [IntegerValidator(MinValue = 0, MaxValue = 25565, ExcludeRange = false)]
    public required int Port { get; init; }

    [StringValidator(MinLength = 1)]
    public required string Username { get; init; }

    [StringValidator(MinLength = 1)]
    public required string Password { get; init; }

    [StringValidator(MinLength = 1)]
    public required string FromAddress { get; init; }

    [StringValidator(MinLength = 1)]
    public required string FromName { get; init; }

    public bool IsSsl => this.Port == 587;
    public MailAddress From => new MailAddress(this.FromAddress, this.FromName);

    public ICredentialsByHost Credentials => new NetworkCredential(this.Username, this.Password);
}
