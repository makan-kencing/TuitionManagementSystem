namespace TuitionManagementSystem.Web.Services.Email;

using System.Configuration;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public sealed class SmtpEmailService : IEmailService, IDisposable
{
    private readonly MailAddress from;
    private readonly SmtpClient client;

    public SmtpEmailService(IConfiguration configuration)
    {
        var host = configuration["SMTP_HOST"]
                   ?? throw new ConfigurationErrorsException("SMTP_HOST is empty.");
        var port = configuration.GetValue<int?>("SMTP_PORT")
            ?? throw new ConfigurationErrorsException("SMTP_PORT is empty or invalid.");
        var username = configuration["SMTP_USERNAME"]
                       ?? throw new ConfigurationErrorsException("SMTP_USERNAME is empty.");
        var password = configuration["SMTP_PASSWORD"]
                       ?? throw new ConfigurationErrorsException("SMTP_PASSWORD is empty.");
        var fromAddress = configuration["SMTP_FROM_ADDRESS"]
                          ?? throw new ConfigurationErrorsException("SMTP_FROM_ADDRESS is empty.");
        var fromName = configuration["SMTP_FROM_NAME"]
                       ?? throw new ConfigurationErrorsException("SMTP_FROM_NAME is empty.");

        this.client = new()
        {
            Host = host,
            Port = port,
            EnableSsl = port == 587,
            Credentials = new NetworkCredential(username, password)
        };
        this.from = new MailAddress(fromAddress, fromName);
    }

    public async Task SendAsync(MailMessage mail, CancellationToken cancellationToken = default)
    {
        mail.From = this.from;
        await this.client.SendMailAsync(
            mail,
            cancellationToken: cancellationToken);
    }

    public void Dispose() =>
        this.client.Dispose();
}
