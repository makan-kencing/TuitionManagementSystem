namespace TuitionManagementSystem.Web.Services.Email;

using System.Configuration;
using System.Net.Mail;
using Configurations;

public sealed class SmtpEmailService : IEmailService, IDisposable
{
    private readonly SmtpOptions options;
    private readonly SmtpClient client;

    public SmtpEmailService(IConfiguration configuration)
    {
        this.options = configuration.GetSection("Smtp")
                           .Get<SmtpOptions>()
                       ?? throw new ConfigurationErrorsException("Smtp not configured");

        this.client = new()
        {
            Host = this.options.Host,
            Port = this.options.Port,
            EnableSsl = this.options.IsSsl,
            Credentials = this.options.Credentials
        };
    }

    public async Task SendAsync(MailMessage mail, CancellationToken cancellationToken = default)
    {
        mail.From = this.options.From;
        await this.client.SendMailAsync(
            mail,
            cancellationToken: cancellationToken);
    }

    public void Dispose() =>
        this.client.Dispose();
}
