using System.Net;
using System.Net.Mail;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using Microsoft.Extensions.Options;

namespace KanjiReader.ExternalServices.EmailSender;

public class EmailSender(IOptionsMonitor<EmailOptions> options)
{
    public async Task SendNotification(string email, GenerationSourceType generationSource)
    {
        var smtp = new SmtpClient(options.CurrentValue.Host, options.CurrentValue.Port)
        {
            Credentials = new NetworkCredential(options.CurrentValue.FromAddress, options.CurrentValue.Password),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        var mail = new MailMessage
        {
            From = new MailAddress(options.CurrentValue.FromAddress),
            Subject = "Message from KanjiReader",
            Body = $"Your japanese texts from {generationSource.ToString()} are ready!"
        };
        mail.To.Add(email);

        await smtp.SendMailAsync(mail);
    }
    public async Task SendConfirmation(string email, string confirmationCode)
    {
        var smtp = new SmtpClient(options.CurrentValue.Host, options.CurrentValue.Port)
        {
            Credentials = new NetworkCredential(options.CurrentValue.FromAddress, options.CurrentValue.Password),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        var mail = new MailMessage
        {
            From = new MailAddress(options.CurrentValue.FromAddress),
            Subject = "Confirm your email",
            Body = $"This is your confirmation code: {confirmationCode}"
        };
        mail.To.Add(email);

        await smtp.SendMailAsync(mail);
    }
}