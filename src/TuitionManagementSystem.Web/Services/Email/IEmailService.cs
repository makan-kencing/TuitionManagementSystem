namespace TuitionManagementSystem.Web.Services.Email;

using System.Net.Mail;

public interface IEmailService
{
    public Task SendAsync(MailMessage mail, CancellationToken cancellationToken = default);
}
